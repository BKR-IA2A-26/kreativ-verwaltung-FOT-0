using System;
using System.Collections.Generic;

namespace Vereinsverwaltung.Models;

public partial class Mannschaften
{
    public int MannschaftsId { get; set; }

    public string Name { get; set; } = null!;

    public string? Liga { get; set; }

    public string? Altersklasse { get; set; }

    public string? Saison { get; set; }

    public virtual ICollection<Mitglieder> Mitglieders { get; set; } = new List<Mitglieder>();

    public virtual ICollection<Spiele> Spieles { get; set; } = new List<Spiele>();

    public virtual ICollection<Trainer> Trainers { get; set; } = new List<Trainer>();
}
