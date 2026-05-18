using System;
using System.Collections.Generic;

namespace Vereinsverwaltung.Models;

public partial class Trainer
{
    public int TrainerId { get; set; }

    public string Vorname { get; set; } = null!;

    public string Nachname { get; set; } = null!;

    public string? Telefonnummer { get; set; }

    public string? Lizenzstufe { get; set; }

    public virtual ICollection<Mannschaften> Mannschafts { get; set; } = new List<Mannschaften>();
}
