using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace NutriCore.src
{
    /// <summary>
    /// Liste zum Laden aller getrackten Daten 
    /// Füllen mit TrackedData
    /// Laden pro Tag
    /// Am Tag Getracktes direkt hinzufügen, um erneutes Laden zu ersparen
    /// Anzeigen in TrackingFenster
    /// 
    /// NICHT ZUM SPEICHERN GEEIGNET - Speichern immer direkt in DB
    /// 
    /// Observable Collection für Anzeige refresh
    /// 
    /// </summary>
    class TrackedEntries<T> : ObservableCollection<TrackedData>
    {
        public void LoadDBEntriesTrackedPerDay(DateOnly chosenDate)
        {
            this.Clear();

            string sql = "SELECT id, referenz_id, name, menge_in_g, getrackt_am " +
                         "FROM tracked_data " +
                         "WHERE getrackt_am = @today";                      

            using (MySqlConnection connection = new MySqlConnection(DBConnection.ServerConnection))
            {
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                cmd.Parameters.AddWithValue("@today", chosenDate.ToDateTime(TimeOnly.MinValue));
                connection.Open();
                using MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    int id = reader.GetInt32("id");
                    string refr = reader.GetString("referenz_id");
                    string name = reader.GetString("name");
                    double qty = reader.GetDouble("menge_in_g");
                    DateOnly track = DateOnly.FromDateTime(reader.GetDateTime("getrackt_am"));
                    
                    this.Add(new TrackedData(id, refr, name, qty, track));
                }
            }  
        }
    }
}
