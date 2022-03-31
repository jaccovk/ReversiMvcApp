using System.ComponentModel.DataAnnotations;

namespace ReversiMvcApp.Models
{
    public class Speler
    {
        [Key]
        public string Guid { get; set; }
        public string Naam { get; set; }
        public int AantalGewonnen{ get; set; }
        public int AantalGelijk{ get; set; }
        public int AantalVerloren{ get; set; }

    }
}
