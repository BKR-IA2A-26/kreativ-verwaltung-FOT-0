using System.Collections.Generic;

namespace Vereinsverwaltung.Models
{
    public partial class Plaetze
    {
        public int PlatzId { get; set; }
        public string? Name { get; set; }
        public string? Adresse { get; set; }
        public string? Typ { get; set; } 

        public string? Status { get; set; } = "Freigegeben";

        public virtual ICollection<Spiele> Spiele { get; set; } = new List<Spiele>();
    }
}