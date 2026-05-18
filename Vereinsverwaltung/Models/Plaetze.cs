using System;
using System.Collections.Generic;

namespace Vereinsverwaltung.Models;

public partial class Plaetze
{
    public int PlatzId { get; set; }

    public string Name { get; set; } = null!;

    public string? Adresse { get; set; }

    public string? Typ { get; set; }

    public bool? Flutlicht { get; set; }

    public virtual ICollection<Spiele> Spieles { get; set; } = new List<Spiele>();
}
