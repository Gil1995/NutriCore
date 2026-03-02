using NutriCore.src;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace NutriCore
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    
    public partial class MainWindow : Window
    {                
        public MainWindow()
        {
         
            InitializeComponent();
            
            //MainViewModel erstellen, um Content zu laden
            DataContext = new MainViewModel();

        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            //öffnet browser
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri)
            {
                UseShellExecute = true
            });

            e.Handled = true;
        }


        [DllImport("dwmapi.dll")]
        static extern int DwmSetWindowAttribute(
        IntPtr hwnd,
        int dwAttribute,
        ref int pvAttribute,
        int cbAttribute);

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;

            // 35 = DWMWA_CAPTION_COLOR
            int DWMWA_CAPTION_COLOR = 35;

            // Farbe im BGR Format (nicht RGB!)
            int color = 0x00302D2D;

            DwmSetWindowAttribute(hwnd, DWMWA_CAPTION_COLOR, ref color, sizeof(int));           
        }


        private void AddLM_Click(object sender, RoutedEventArgs e)
        {
            AddFoodWindow add = new AddFoodWindow();
            add.Owner = this;
            add.DataContext = this.DataContext;
            add.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            add.ShowDialog();
        }

        private void TrackingListbox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataContext is MainViewModel vm && vm.SelectedEntry != null)
            {
                vm.EditCommand.Execute(vm.SelectedEntry);

                vm.SelectedEntry = null;
            }
        }

        private void ImpressumTextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string titel = "Impressum";
            string text = "Verantwortlich für den Inhalt: \nIsabella Franziska Herold\n\n" +
                "Alle Inhalte zur nicht-kommerziellen Nutzung " +
                "\nFont (Faith): https://www.dafont.com/faith-2.font \nBild (Icon): https://media.istockphoto.com/";
            MessageBox.Show(text,titel);
        }
    }
}