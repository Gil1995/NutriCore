using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;

namespace NutriCore.src
{
    internal class BLSEntries<T> : ObservableCollection<T>
    {
        public BLSEntries<EssentialData> LoadDBEntriesBLS()
        {
            string sql = "SELECT bls_code, lebensmittelbezeichnung, enercj_energie_kjoule_pro_100g, "+
                         "enercc_energie_kcal_pro_100g, fat_fett_g_pro_100g, cho_kohlenhydrate_g_pro_100g, "+
                         "sugar_zucker_ges_g_pro_100g, fibt_ballaststoffe_ges_g_pro_100g, "+
                         "prot625_protein_g_pro_100g, nacl_salz_g_pro_100g "+
                         "FROM essential_data";

            BLSEntries<EssentialData> entriesList = new BLSEntries<EssentialData>();

            using (MySqlConnection connection = new MySqlConnection(DBConnection.ServerConnection))
            {
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                connection.Open();
                using MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string blsCode = reader.GetString("bls_code");
                    string foodName = reader.GetString("lebensmittelbezeichnung");
                    double enercj = GetSafeDouble(reader,"enercj_energie_kjoule_pro_100g");
                    double enercc = GetSafeDouble(reader, "enercc_energie_kcal_pro_100g");
                    double fat = GetSafeDouble(reader, "fat_fett_g_pro_100g");
                    double cho = GetSafeDouble(reader, "cho_kohlenhydrate_g_pro_100g");
                    double sugar = GetSafeDouble(reader, "sugar_zucker_ges_g_pro_100g");
                    double fibt = GetSafeDouble(reader, "fibt_ballaststoffe_ges_g_pro_100g");
                    double prot625 = GetSafeDouble(reader, "prot625_protein_g_pro_100g");
                    double nacl = GetSafeDouble(reader, "nacl_salz_g_pro_100g");



                    entriesList.Add(new EssentialData
                        (blsCode, foodName, enercj, enercc, fat, cho, sugar, fibt, prot625, nacl));
                }
            }
            return entriesList;
        }

        public static double GetSafeDouble(MySqlDataReader reader, string columnName)
        {

            //Absicherung für null Werte in DB
            if (reader.IsDBNull(reader.GetOrdinal(columnName)))
                return 0;

            //Object -> gilt für alles was der reader ziehen könnte
            object value = reader[columnName];

            string strValue = value.ToString();
            
            //Parse des strings auf Zahlen (Culture DE, also Komma als Trennzeichen)
            if (double.TryParse(strValue,
                                System.Globalization.NumberStyles.Any,
                                System.Globalization.CultureInfo.GetCultureInfo("de-DE"),
                                out double result))
            {
                return result;
            }

            //Bei Misserfolg des Parse (LOD/LOQ) -> Wert wird 0
            return 0;
        }

    }
}
