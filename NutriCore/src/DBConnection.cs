namespace NutriCore.src
{
    
    internal class DBConnection
    {
        private static readonly string _serverConnection = "Server=localhost;Database=nutricore;User ID=root;Password=WasGehtSieDasAN;";

        public static string ServerConnection => _serverConnection;
    }
}
