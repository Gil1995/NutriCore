using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace NutriCore.src
{
    internal class TrackingGoalsCalc : INotifyPropertyChanged 
    {
        private double _kcal;
        private double _cho;
        private double _prot;
        private double _fat;
        private double _fiber;
        
        Dictionary<EssentialData, int> trackedFoodsPerDay = new Dictionary<EssentialData, int>();

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        internal Dictionary<EssentialData, int> TrackedFoodsPerDay { get => trackedFoodsPerDay; set => trackedFoodsPerDay = value; }
        
        public double Kcal
        {
            get => _kcal;
            set
            {
                if (_kcal != value)
                {
                    _kcal = value;
                    OnPropertyChanged(nameof(Kcal));
                }
            }
        }

        public double Cho
        {
            get => _cho;
            set
            {
                if (_cho != value)
                {
                    _cho = value;
                    OnPropertyChanged(nameof(Cho));
                }
            }
        }

        public double Prot
        {
            get => _prot;
            set
            {
                if (_prot != value)
                {
                    _prot = value;
                    OnPropertyChanged(nameof(Prot));
                }
            }
        }

        public double Fat
        {
            get => _fat;
            set
            {
                if (_fat != value)
                {
                    _fat = value;
                    OnPropertyChanged(nameof(Fat));
                }
            }
        }

        public double Fiber
        {
            get => _fiber;
            set
            {
                if (_fiber != value)
                {
                    _fiber = value;
                    OnPropertyChanged(nameof(Fiber));
                }
            }
        }


        //Wenn ein Objekt erstellt wird für den betreffenden Tag -> wird auch direkt alles aus der DB für den Tag gezogen
        public TrackingGoalsCalc(DateOnly dateChosen)
        {
            //Liste füllen - alle getrackten Lebensmittel des Tages
            FillListForCurrentDay(dateChosen);
            //wenn keine einträge -> leeres objekt
            if (TrackedFoodsPerDay == null)
                return;

            foreach (var entry in TrackedFoodsPerDay)
            {
                EssentialData food = entry.Key;
                int grams = entry.Value;
                double factor = grams / 100.0;

                //wenn der gewählte Tag bereits Einträge hat -> werden die Makros hier aufsummiert & gerundet
                Kcal += Math.Round( food.Enercc * factor );
                Cho += Math.Round( food.Cho * factor );
                Fat += Math.Round( food.Fat * factor );
                Prot += Math.Round( food.Prot625 * factor );
                Fiber += Math.Round( food.Fibt * factor );               
            }
        }

        //Zieht TrackedData aus DB für das gewählte Datum
        private void FillListForCurrentDay(DateOnly date)
        {
            //löscht bestehende liste, um nur das gewählte Datum in Liste zu haben
            TrackedFoodsPerDay.Clear();

            string sql = "SELECT e.*, t.menge_in_g " +
                         "FROM essential_data e " +
                         "INNER JOIN tracked_data t " +
                         "ON e.bls_code = t.referenz_id " +
                         "WHERE t.getrackt_am = @datum;";

            using (MySqlConnection connection = new MySqlConnection(DBConnection.ServerConnection))
            {
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@datum", date.ToDateTime(TimeOnly.MinValue));
                connection.Open();
                using MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string blsCode = reader.GetString("bls_code");
                    string foodName = reader.GetString("lebensmittelbezeichnung");
                    double enercj = BLSEntries<EssentialData>.GetSafeDouble(reader, "enercj_energie_kjoule_pro_100g");
                    double enercc = BLSEntries<EssentialData>.GetSafeDouble(reader, "enercc_energie_kcal_pro_100g");
                    double fat = BLSEntries<EssentialData>.GetSafeDouble(reader, "fat_fett_g_pro_100g");
                    double cho = BLSEntries<EssentialData>.GetSafeDouble(reader, "cho_kohlenhydrate_g_pro_100g");
                    double sugar = BLSEntries<EssentialData>.GetSafeDouble(reader, "sugar_zucker_ges_g_pro_100g");
                    double fibt = BLSEntries<EssentialData>.GetSafeDouble(reader, "fibt_ballaststoffe_ges_g_pro_100g");
                    double prot625 = BLSEntries<EssentialData>.GetSafeDouble(reader, "prot625_protein_g_pro_100g");
                    double nacl = BLSEntries<EssentialData>.GetSafeDouble(reader, "nacl_salz_g_pro_100g");
                    
                    int quantTracked = reader.GetInt32("menge_in_g");
                                        
                    var essentialData = new EssentialData(
                        blsCode, foodName, enercj, enercc, fat, cho, sugar, fibt, prot625, nacl);

                    //Absicherung für Doppelte Lebensmitteleinträge, Wenn nutzer 2 Instanzen am Tag trackt zb (2x Huhn je 150g -> Huhn, 300g)                   
                    TrackedFoodsPerDay[essentialData] =
                                           //wenn Key bereits vorhanden -> value, sonst 0
                        TrackedFoodsPerDay.GetValueOrDefault(essentialData) + quantTracked;
                }
            }
        }     




        /*
        private double ParseDoubleOrZero(string? value)
        {
            //Wenn DB kein eintrag = 0 
            if (string.IsNullOrWhiteSpace(value))
                return 0;
            //wenn DB zahleneintrag -> zahlenwert (egal ob , oder . getrennt) - wenn keine zahl -> 0 (LOQ & LOD werte) 
            return double.TryParse(value.Replace(",", "."), System.Globalization.NumberStyles.Any,
                                   System.Globalization.CultureInfo.InvariantCulture, out double result)
                ? result
                : 0;
        }*/

    }
}
