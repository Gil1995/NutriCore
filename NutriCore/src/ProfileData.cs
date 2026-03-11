using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace NutriCore.src
{
    /// <summary>
    /// Vom Nutzer eingegebene Start-Daten
    /// Werden in .txt gespeichert
    /// </summary>
    internal class ProfileData 
    {
        ///Persönliche Daten
        private string _name;
        private double _startingWeight;
        private double _weightGoal;
        private string _motivation;
        ///Tracking Ziele / Max Makros
        private int _kcalMax;
        private int _choMax;
        private int _protMax;
        private int _fatMax;
        private int _fiberMax;
        private int _waterMax;

        public ProfileData()
        {
            Name = "-";
            StartingWeight = 0;
            WeightGoal = 0;
            Motivation = "-";

            KcalMax = 0;
            ChoMax = 0;
            ProtMax = 0;
            FatMax = 0;
            FiberMax = 0;
            // WaterMax = 0;
        }
        // --------------------------------------------- Konstruktoren ---------------------------------------------------------------------------------------- //
        public ProfileData(string name, double weightStart, double weightGoal, string motiviation,
                            int kcal, int cho, int prot, int fat, int fiber) //,int water)
        {
            Name = name;
            StartingWeight = weightStart;
            WeightGoal = weightGoal;
            Motivation = motiviation;

            KcalMax = kcal;
            ChoMax = cho;
            ProtMax = prot;
            FatMax = fat;
            FiberMax = fiber;
            // WaterMax = water;
        }

        ///Load für Systemstart
        public ProfileData GetProfileDataFromFile()
        {
            string fullPath = GetProfileFilePath();

            if (!File.Exists(fullPath))
            {
                return new ProfileData();
            }

            string json = File.ReadAllText(fullPath);
            ProfileData? data = JsonSerializer.Deserialize<ProfileData>(json);

            return data ?? new ProfileData();
        }

        // ----------------------------------------- Methoden ----------------------------------------------------------------------------------------------- //

        //Speichern in txt (Use on tab leave ! - Safety falls Programm abstürzt oder unsachgemäß beendet wird)
        public void SafeProfileData(ProfileData toSave)
        {
            string fullPath = GetProfileFilePath();
            string json = JsonSerializer.Serialize(toSave);

            try
            {
                File.WriteAllText(fullPath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Speichern: {ex.Message}");
            }
        }

        private string GetProfileFilePath()
        {
            string basePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "NutriCore",
                "resources");

            Directory.CreateDirectory(basePath); 

            string fileName = "ProfileData.txt"; 
            return Path.Combine(basePath, fileName);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        //------------------------------------------ Getter&Setter -------------------------------------------------------------------------------------- //
        public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }
        public double StartingWeight { get => _startingWeight; set { _startingWeight = value; OnPropertyChanged(); } }
        public double WeightGoal { get => _weightGoal; set { _weightGoal = value; OnPropertyChanged(); } }
        public string Motivation { get => _motivation; set { _motivation = value; OnPropertyChanged(); } }
        public int KcalMax { get => _kcalMax; set { _kcalMax = value; OnPropertyChanged(); } }
        public int ChoMax { get => _choMax; set { _choMax = value; OnPropertyChanged(); } }
        public int ProtMax { get => _protMax; set { _protMax = value; OnPropertyChanged(); } }
        public int FatMax { get => _fatMax; set { _fatMax = value; OnPropertyChanged(); } }
        public int FiberMax { get => _fiberMax; set { _fiberMax = value; OnPropertyChanged(); } }
        // public int WaterMax { get => _waterMax; set { _waterMax = value; OnPropertyChanged(); } }
    }
}
