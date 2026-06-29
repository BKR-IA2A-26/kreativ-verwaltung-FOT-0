using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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

    public int? Trainer_ID { get; set; }

    [ForeignKey("Trainer_ID")]
    public virtual Trainer? Trainer { get; set; }
}
