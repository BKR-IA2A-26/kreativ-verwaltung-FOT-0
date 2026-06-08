using System;

namespace Vereinsverwaltung.Models
{
    public partial class Trainingszeiten
    {
        public int TrainingId { get; set; }
        public int MannschaftId { get; set; }
        public int PlatzId { get; set; }
        public string Wochentag { get; set; } = null!;
        public TimeOnly Startzeit { get; set; }
        public TimeOnly Endzeit { get; set; }

        public virtual Mannschaften? Mannschafts { get; set; }
        public virtual Plaetze? Platz { get; set; }
    }
}