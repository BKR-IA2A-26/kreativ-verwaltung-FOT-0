using System.Collections.Generic;

namespace Vereinsverwaltung.Models
{
    public partial class Plaetze
    {
        public int PlatzId { get; set; }
        public string? Name { get; set; }
        public string? Adresse { get; set; }
        public string? Typ { get; set; } // z.B. "Rasen", "Kunstrasen", "Halle"

        public string? Status { get; set; } = "Freigegeben";
        // Navigationseigenschaft zu den Spielen
        public virtual ICollection<Spiele> Spiele { get; set; } = new List<Spiele>();
    }
}