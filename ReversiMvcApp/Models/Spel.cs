namespace ReversiMvcApp.Models
{
    public class Spel
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public string Speler1Token { get; set; }
        public string Speler2Token { get; set; }
        public string Omschrijving{ get; set; }
        public string Winnaar { get; set; }
        public bool Afgelopen { get; set; }
        public string AandeBeurt { get; set; }
    }
}
