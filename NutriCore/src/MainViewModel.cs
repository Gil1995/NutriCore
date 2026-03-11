using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Printing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NutriCore.src
{
    /// <summary>
    /// MVVM: ViewModel fürs MainWindow
    /// Schnittstelle zwischen CodeBehind & UI
    /// </summary>
    internal class MainViewModel : INotifyPropertyChanged
    {
        private ProfileData _user = new ProfileData();
        private DateOnly chosenDate;
        private TrackedEntries<TrackedData> dbEntriesTracked = new TrackedEntries<TrackedData>();
        private readonly BLSEntries<EssentialData> _blsDatabase;
        private BLSEntries<EssentialData> _searchedEntries = new BLSEntries<EssentialData>();
        private string searchInput;
        private EssentialData selectedFood;
        private Visibility _makroDetailsVisibility = Visibility.Collapsed;
        private TrackedData? _selectedEntry;

        // -------------------------------------------------------- PROPERTIES -----------------------------------------------------------------------//
        public ProfileData User
        {
            get => _user;
            set
            {
                _user = value;
                //Da ProfileData anders als die Entries keine ObservableCollection o.ä. ist, muss UI über changes informiert werden
                OnPropertyChanged();
            }
        }
   
        public BLSEntries<EssentialData> DBEntriesBLS => _blsDatabase;
        public BLSEntries<EssentialData> SearchedEntries
        {
            get => _searchedEntries;
            set
            {               
                    _searchedEntries = value;
                    OnPropertyChanged(); 
            }
        }
        public TrackedEntries<TrackedData> DBEntriesTracked { get => dbEntriesTracked; set => dbEntriesTracked = value; }
        public ICommand EditCommand { get; }
        public ICommand ShowMakroCommand { get; }
        public ICommand ProfileTabChangedCommand { get; }
        public ICommand WindowClosingCommand { get; }
        public ICommand AddAndCloseCommand { get; }
        public TrackingGoalsCalc TrackingVar { get; set; }       

        public TrackedData? SelectedEntry
        {
            get => _selectedEntry;
            set
            {
                if (_selectedEntry == value)
                    return;

                _selectedEntry = value;
                OnPropertyChanged();
            }
        }
        public EssentialData SelectedFood
        {
            get => selectedFood;
            set
            {
                if (selectedFood != value)
                {
                    selectedFood = value;
                    OnPropertyChanged();                   
                    MakroDetailsVisibility = selectedFood != null ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }
        public DateOnly ChosenDate
        {
            get => chosenDate;
            set
            {
                if (chosenDate != value)
                {
                    chosenDate = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ChosenDateDateTime));
                    RefreshTrackedEntries();
                    TrackingVar = new TrackingGoalsCalc(chosenDate);
                    OnPropertyChanged(nameof(TrackingVar));
                }
            }
        }
        public DateTime? ChosenDateDateTime
        {
            get => ChosenDate.ToDateTime(TimeOnly.MinValue);

            set
            {
                if (value.HasValue)
                {
                    ChosenDate = DateOnly.FromDateTime(value.Value);
                }
            }
        }
        public string SearchInput
        {
            get => searchInput;
            set
            {
                if (searchInput != value)
                {
                    searchInput = value;
                    OnPropertyChanged();
                }
            }
        }
        public ICommand SendSearchCommand => new RelayCommand<object>(_ => ExecuteSearch());

        public Visibility MakroDetailsVisibility
        {
            get => _makroDetailsVisibility;
            set
            {
                if (_makroDetailsVisibility != value)
                {
                    _makroDetailsVisibility = value;
                    OnPropertyChanged();
                }
            }
        }
        // ------------------------------------------------------------- KONSTRUKTOR ----------------------------------------------------------------------------//
        public MainViewModel() 
        {
            //Datenbank BLS Laden|Liste füllen
            _blsDatabase = new BLSEntries<EssentialData>().LoadDBEntriesBLS();

           
            //initialisieren, damit DatePicker nicht bei 01.01.0001 startet sondern immer beim aktuellen Tag
            chosenDate = DateOnly.FromDateTime(DateTime.Today);

            SearchedEntries = new BLSEntries<EssentialData>();           

            DBEntriesTracked = new TrackedEntries<TrackedData>();
            RefreshTrackedEntries();
            

            EditCommand = new RelayCommand<TrackedData>(EditTrackedData);


            //User Profil laden
            User = new ProfileData().GetProfileDataFromFile();
            //UserProfil bei TabWechsel speichern
            ProfileTabChangedCommand = new RelayCommand<object>(OnProfileTabChanged);
            //UserProfil bei Programmende speichern
            WindowClosingCommand = new RelayCommand<object>(_ => SaveProfile());
            //Befehl um Lebensmittel hinzuzufügen
            AddAndCloseCommand = new RelayCommand<object>(ExecuteAdd);
        }
        

        // ------------------------------------------------------------- HILFSMETHODEN ------------------------------------------------------------------------//

        //Methode zum Suchen von Lebensmitteln per Texteingabe & Suchbutton
        private void ExecuteSearch()
        {
            if (!string.IsNullOrWhiteSpace(SearchInput))
                SearchedEntries = SearchInList(SearchInput);
            else
                SearchedEntries = new BLSEntries<EssentialData>();
        }

        //Aktualisiert die Anzeige der getrackten Lebensmittel
        private void RefreshTrackedEntries()
        {
            DBEntriesTracked.LoadDBEntriesTrackedPerDay(chosenDate);
            TrackingVar = new TrackingGoalsCalc(ChosenDate);
            OnPropertyChanged(nameof(TrackingVar));
        }

        //Wenn der Profil-Tab verlassen wird, werden die eingetragenen Daten gespeichert
        private void OnProfileTabChanged(object parameter)
        {
            
            if (parameter is SelectionChangedEventArgs e)
            {
                if (e.RemovedItems.Count > 0)
                {
                    var removedTab = e.RemovedItems[0] as TabItem;

                    if (removedTab?.Header?.ToString() == "Profil")
                    {
                        SaveProfile();
                    }
                }
            }
        }

        //Um getrackte Daten zu bearbeiten 
        private void EditTrackedData(TrackedData item)
        {
            EditFoodWindow edit = new EditFoodWindow(item);
            edit.Owner = Application.Current.MainWindow;
            edit.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            bool? closed = edit.ShowDialog();

            if (closed == true)
            {
                RefreshTrackedEntries();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        //speichert Profiltab eingaben in ProfileTextDatei
        private void SaveProfile()
        {
            User.SafeProfileData(User);
        }

        //Suchmethode - in DB Einträgen nach Namen suchen
        private BLSEntries<EssentialData> SearchInList(string searchKey)
        {
            BLSEntries<EssentialData> searchResults = new BLSEntries<EssentialData>();

            if (string.IsNullOrWhiteSpace(searchKey))
                return searchResults;

            foreach (EssentialData ed in DBEntriesBLS)
            {
                string text = ed.FoodName ?? "";

                if (text.Contains(searchKey, StringComparison.OrdinalIgnoreCase))
                {
                    searchResults.Add(ed);
                }
            }

            return searchResults;
        }

        //Fügt Lebensmittel hinzu
        private void ExecuteAdd(object parameter)
        {
            var activeWindow = Application.Current.Windows
                                .OfType<Window>()
                                .SingleOrDefault(x => x.IsActive);
            //sollte nicht eintreten können, da mengen textbox erst bei auswahl gezeigt - but safety first
            if (SelectedFood == null)
            {                
                MessageBox.Show(
                    Application.Current.MainWindow,
                    "Bitte zuerst ein Lebensmittel auswählen.",
                    "Keine Auswahl",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return; 
            }

            //wenn keine Menge eingegeben wurde
            if (!(parameter is string mengeText) || string.IsNullOrWhiteSpace(mengeText))
            {
                MessageBox.Show(
                                activeWindow,
                                "Bitte eine Menge eingeben.",
                                "Fehlende Eingabe",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            //wenn text statt zahl eingegeben wurde
            if (!double.TryParse(mengeText, out double menge))
            {
                MessageBox.Show(
                                activeWindow,       
                                "Bitte eine gültige Zahl eingeben.",
                                "Ungültige Eingabe",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            TrackedData toTrack = new TrackedData(
                SelectedFood.BlsCode,
                SelectedFood.FoodName,
                menge,
                ChosenDate);

            toTrack.SafeTrackedDataInDB(toTrack);
            RefreshTrackedEntries();
            MakroDetailsVisibility = Visibility.Collapsed;

        }
    }
}
