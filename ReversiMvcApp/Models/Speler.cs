using System.ComponentModel.DataAnnotations;

namespace ReversiMvcApp.Models
{
    public class Speler
    {
        [Key]
        public string Guid { get; set; }
        public string Naam { get; set; }
        public int AantalGewonnen { get; set; } = 0;
        public int AantalGelijk { get; set; } = 0;
        public int AantalVerloren { get; set; } = 0;

    }
}
