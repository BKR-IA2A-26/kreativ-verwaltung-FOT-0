using System;
using System.Collections.Generic;

namespace Vereinsverwaltung.Models;

public partial class Spiele
{
    public int SpielId { get; set; }

    public int MannschaftsId { get; set; }

    public int? PlatzId { get; set; }

    public DateOnly? Datum { get; set; }

    public TimeOnly? Uhrzeit { get; set; }

    public string? Gegner { get; set; }

    public string? HeimAuswaerts { get; set; }

    public string? Ergebnis { get; set; }

    public string? Spielort { get; set; }

    public virtual Mannschaften Mannschafts { get; set; } = null!;

    public virtual Plaetze? Platz { get; set; }
}
