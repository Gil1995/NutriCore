using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;

namespace NutriCore.src
{
    /// <summary>
    /// Objekt um einen getrackten Eintrag aus der DB abzubilden
    /// </summary>
    public class TrackedData : INotifyPropertyChanged
    {
        private int _tableID;
        private string _referenceID;      
        private string _name;
        private double _quantityTracked;
        private DateOnly _dayOfTracking;

        // ----------------------------------- Konstruktoren --------------------------------------------------------------------- //

        //Konstruktor um Daten für DB vorzubereiten (DB vergibt ID, daher id=0 -> und in Safe methode nicht übergeben)
        public TrackedData(string _referenceID, string _name, double _quantityTracked, DateOnly _dayOfTracking)
        {
            this.TableID = 0;
            this.ReferenceID = _referenceID;
            this.Name = _name;
            this.QuantityTracked = _quantityTracked;
            this.DayOfTracking = _dayOfTracking;
        }
        
        //Konstruktor um Daten aus DB abzubilden
        public TrackedData(int _tableID, string _referenceID, string _name, double _quantityTracked, DateOnly _dayOfTracking)
        {
            this.TableID = _tableID;
            this.ReferenceID = _referenceID;
            this.Name = _name;
            this.QuantityTracked = _quantityTracked;
            this.DayOfTracking = _dayOfTracking;
        } 

        // ---------------------------------------- Methoden --------------------------------------------------------------------------//

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public void SafeTrackedDataInDB(TrackedData userInput)
        {
            string id = userInput.ReferenceID;
            string name = userInput.Name;
            double quant = userInput.QuantityTracked;
            DateOnly trackingdate = userInput.DayOfTracking;

            string sql = "INSERT INTO tracked_data (referenz_id, name, menge_in_g, getrackt_am) " +
                         "VALUES (@id, @name, @quant, @trackingdate);";
            using (MySqlConnection connection = new MySqlConnection(DBConnection.ServerConnection))
            {
                MySqlCommand cmd = new MySqlCommand(sql, connection);

                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@quant", quant);
                cmd.Parameters.AddWithValue("@trackingdate", trackingdate.ToString("yyyy-MM-dd"));

                connection.Open();
                cmd.ExecuteNonQuery();
            }
            MessageBox.Show("Lebensmittel getrackt");           
        }
        // -------------------------------------------- Getter & Setter -----------------------------------------------------------------//
        public string ShortenName
        {
            get
            {
                if (Name.Length > 25)
                {
                    return Name.Substring(0, 25) + "... " ;
                }
                return Name;
            }
        }

        public string UnifyQuantity
        {
            get
            {
                return QuantityTracked + "g";
            }
        }
        public int TableID { get => _tableID; set => _tableID = value; }
        public string ReferenceID { get => _referenceID; set => _referenceID = value; }
        public double QuantityTracked
        {
            get => _quantityTracked;
            set
            {
                _quantityTracked = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(UnifyQuantity));
            }
        }
        public DateOnly DayOfTracking { get => _dayOfTracking; set => _dayOfTracking = value; }
        public string Name { get => _name; set => _name = value; }
    }
}
