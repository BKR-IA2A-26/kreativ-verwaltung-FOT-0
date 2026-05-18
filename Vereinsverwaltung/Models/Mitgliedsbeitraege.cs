using System;
using System.Collections.Generic;

namespace Vereinsverwaltung.Models;

public partial class Mitgliedsbeitraege
{
    public int BeitragId { get; set; }

    public int MitgliederId { get; set; }

    public decimal? Betrag { get; set; }

    public string? Rhythmus { get; set; }

    public string? Zahlungsstatus { get; set; }

    public DateOnly? Faelligkeitsdatum { get; set; }

    public virtual Mitglieder Mitglieder { get; set; } = null!;
}
