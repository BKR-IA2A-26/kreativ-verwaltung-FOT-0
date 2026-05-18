using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace Vereinsverwaltung.Models;

public partial class VereinsverwaltungContext : DbContext
{
    public VereinsverwaltungContext()
    {
    }

    public VereinsverwaltungContext(DbContextOptions<VereinsverwaltungContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Mannschaften> Mannschaftens { get; set; }

    public virtual DbSet<Mitglieder> Mitglieders { get; set; }

    public virtual DbSet<Mitgliedsbeitraege> Mitgliedsbeitraeges { get; set; }

    public virtual DbSet<Plaetze> Plaetzes { get; set; }

    public virtual DbSet<Spiele> Spieles { get; set; }

    public virtual DbSet<Trainer> Trainers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;port=3306;user=root;database=vereinsverwaltung", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.4.3-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Mannschaften>(entity =>
        {
            entity.HasKey(e => e.MannschaftsId).HasName("PRIMARY");

            entity.ToTable("mannschaften");

            entity.Property(e => e.MannschaftsId).HasColumnName("Mannschafts_ID");
            entity.Property(e => e.Altersklasse).HasMaxLength(50);
            entity.Property(e => e.Liga).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Saison).HasMaxLength(20);
        });

        modelBuilder.Entity<Mitglieder>(entity =>
        {
            entity.HasKey(e => e.MitgliederId).HasName("PRIMARY");

            entity.ToTable("mitglieder");

            entity.HasIndex(e => e.MannschaftsId, "Mannschafts_ID");

            entity.Property(e => e.MitgliederId).HasColumnName("Mitglieder_ID");
            entity.Property(e => e.Adresse).HasMaxLength(255);
            entity.Property(e => e.EMail)
                .HasMaxLength(100)
                .HasColumnName("E_Mail");
            entity.Property(e => e.MannschaftsId).HasColumnName("Mannschafts_ID");
            entity.Property(e => e.Nachname).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(20);
            entity.Property(e => e.Vorname).HasMaxLength(50);

            entity.HasOne(d => d.Mannschafts).WithMany(p => p.Mitglieders)
                .HasForeignKey(d => d.MannschaftsId)
                .HasConstraintName("mitglieder_ibfk_1");
        });

        modelBuilder.Entity<Mitgliedsbeitraege>(entity =>
        {
            entity.HasKey(e => e.BeitragId).HasName("PRIMARY");

            entity.ToTable("mitgliedsbeitraege");

            entity.HasIndex(e => e.MitgliederId, "Mitglieder_ID");

            entity.Property(e => e.BeitragId).HasColumnName("Beitrag_ID");
            entity.Property(e => e.Betrag).HasPrecision(6, 2);
            entity.Property(e => e.MitgliederId).HasColumnName("Mitglieder_ID");
            entity.Property(e => e.Rhythmus).HasMaxLength(20);
            entity.Property(e => e.Zahlungsstatus).HasMaxLength(20);

            entity.HasOne(d => d.Mitglieder).WithMany(p => p.Mitgliedsbeitraeges)
                .HasForeignKey(d => d.MitgliederId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("mitgliedsbeitraege_ibfk_1");
        });

        modelBuilder.Entity<Plaetze>(entity =>
        {
            entity.HasKey(e => e.PlatzId).HasName("PRIMARY");

            entity.ToTable("plaetze");

            entity.Property(e => e.PlatzId).HasColumnName("Platz_ID");
            entity.Property(e => e.Adresse).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Typ).HasMaxLength(50);
        });

        modelBuilder.Entity<Spiele>(entity =>
        {
            entity.HasKey(e => e.SpielId).HasName("PRIMARY");

            entity.ToTable("spiele");

            entity.HasIndex(e => e.MannschaftsId, "Mannschafts_ID");

            entity.HasIndex(e => e.PlatzId, "Platz_ID");

            entity.Property(e => e.SpielId).HasColumnName("Spiel_ID");
            entity.Property(e => e.Ergebnis).HasMaxLength(10);
            entity.Property(e => e.Gegner).HasMaxLength(100);
            entity.Property(e => e.HeimAuswaerts)
                .HasMaxLength(20)
                .HasColumnName("Heim_Auswaerts");
            entity.Property(e => e.MannschaftsId).HasColumnName("Mannschafts_ID");
            entity.Property(e => e.PlatzId).HasColumnName("Platz_ID");
            entity.Property(e => e.Spielort).HasMaxLength(100);
            entity.Property(e => e.Uhrzeit).HasColumnType("time");

            entity.HasOne(d => d.Mannschafts).WithMany(p => p.Spieles)
                .HasForeignKey(d => d.MannschaftsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("spiele_ibfk_1");

            entity.HasOne(d => d.Platz).WithMany(p => p.Spieles)
                .HasForeignKey(d => d.PlatzId)
                .HasConstraintName("spiele_ibfk_2");
        });

        modelBuilder.Entity<Trainer>(entity =>
        {
            entity.HasKey(e => e.TrainerId).HasName("PRIMARY");

            entity.ToTable("trainer");

            entity.Property(e => e.TrainerId).HasColumnName("Trainer_ID");
            entity.Property(e => e.Lizenzstufe).HasMaxLength(50);
            entity.Property(e => e.Nachname).HasMaxLength(50);
            entity.Property(e => e.Telefonnummer).HasMaxLength(50);
            entity.Property(e => e.Vorname).HasMaxLength(50);

            entity.HasMany(d => d.Mannschafts).WithMany(p => p.Trainers)
                .UsingEntity<Dictionary<string, object>>(
                    "TrainerMannschaften",
                    r => r.HasOne<Mannschaften>().WithMany()
                        .HasForeignKey("MannschaftsId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("trainer_mannschaften_ibfk_2"),
                    l => l.HasOne<Trainer>().WithMany()
                        .HasForeignKey("TrainerId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("trainer_mannschaften_ibfk_1"),
                    j =>
                    {
                        j.HasKey("TrainerId", "MannschaftsId")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("trainer_mannschaften");
                        j.HasIndex(new[] { "MannschaftsId" }, "Mannschafts_ID");
                        j.IndexerProperty<int>("TrainerId").HasColumnName("Trainer_ID");
                        j.IndexerProperty<int>("MannschaftsId").HasColumnName("Mannschafts_ID");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
