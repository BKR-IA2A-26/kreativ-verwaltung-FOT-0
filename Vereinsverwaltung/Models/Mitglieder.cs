using System;
using System.Collections.Generic;

namespace Vereinsverwaltung.Models;

public partial class Mitglieder
{
    public int Mitglieder_Id { get; set; }

    public string Vorname { get; set; } = null!;

    public string Nachname { get; set; } = null!;

    public string? EMail { get; set; }

    public DateOnly? Beitrittsdatum { get; set; }

    public DateOnly? Geburtsdatum { get; set; }

    public string? Adresse { get; set; }

    public string? Status { get; set; }

    public int? MannschaftsId { get; set; }

    public virtual Mannschaften? Mannschaft { get; set; }

    public virtual ICollection<Mitgliedsbeitraege> Mitgliedsbeitraeges { get; set; } = new List<Mitgliedsbeitraege>();


}
