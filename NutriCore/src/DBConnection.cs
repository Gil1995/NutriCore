namespace NutriCore.src
{
    /// <summary>
    /// Datenbank Verbindung 
    /// Pfad & Passwort
    /// </summary>
    internal class DBConnection
    {
        private static readonly string _serverConnection = "Server=localhost;Database=nutricore;User ID=root;Password=Kaffeevollautomat30#;";

        public static string ServerConnection => _serverConnection;
    }
}
