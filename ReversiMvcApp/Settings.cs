namespace ReversiMvcApp
{
    public static class Settings
    {
        private static readonly bool _isProduction = false;
        public static string Url => _isProduction ? 
            "https://localhost:44326/api/Spel" : 
            "http://localhost:3000/api/Spel";

    }
}
