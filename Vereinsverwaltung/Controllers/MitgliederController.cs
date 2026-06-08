using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vereinsverwaltung.Models;
using System.Linq;

namespace Vereinsverwaltung.Controllers
{
    public class MitgliederController : Controller
    {
        private readonly VereinsverwaltungContext _context;

        // Datenbankverbindung über Dependency Injection
        public MitgliederController(VereinsverwaltungContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. READ (Anzeige der Liste mit Such- & Filteroption)
        // ==========================================
        public IActionResult Index(string searchString, int? mannschaftId, string status)
        {
            // 1. Mannschaften für das Filter-Dropdown laden
            ViewBag.Mannschaften = _context.Mannschaften?.ToList() ?? new List<Mannschaften>();

            // Aktuelle Filterwerte in der ViewBag speichern, damit die Filterfelder ausgefüllt bleiben
            ViewBag.CurrentSearch = searchString;
            ViewBag.CurrentMannschaft = mannschaftId;
            ViewBag.CurrentStatus = status;

            // 2. Basis-Abfrage erstellen (wichtig: .Include für das Team)
            var spielerQuery = _context.Mitglieder.Include(m => m.Mannschaft).AsQueryable();

            // 3. Filter: Namenssuche (Vorname oder Nachname)
            if (!string.IsNullOrEmpty(searchString))
            {
                spielerQuery = spielerQuery.Where(m => m.Vorname.Contains(searchString) ||
                                                      m.Nachname.Contains(searchString));
            }

            // 4. Filter: Mannschafts-Zuweisung
            if (mannschaftId.HasValue && mannschaftId.Value > 0)
            {
                spielerQuery = spielerQuery.Where(m => m.MannschaftsId == mannschaftId);
            }

            // 5. Filter: Aktiv / Passiv Status
            if (!string.IsNullOrEmpty(status))
            {
                spielerQuery = spielerQuery.Where(m => m.Status == status);
            }

            // Am Ende die gefilterte Liste an die View übergeben
            return View(spielerQuery.ToList());
        }

        // ==========================================
        // 2. CREATE (Neuen Spieler anlegen)
        // ==========================================

        // GET: Mitglieder/Create (Formular anzeigen)
        [HttpGet]
        public IActionResult Create()
        {
            try
            {
                // Wir holen die Mannschaften. Falls _context.Mannschaften null ist,
                // nutzen wir den ?? Operator, um eine leere Liste zu erzeugen.
                var listenDaten = _context.Mannschaften?.ToList();
                ViewBag.Mannschaften = listenDaten ?? new List<Mannschaften>();
            }
            catch (Exception ex)
            {
                // Falls die DB-Verbindung komplett blockiert (z.B. XAMPP aus), 
                // fangen wir den Fehler ab, damit die Seite trotzdem lädt.
                ViewBag.Mannschaften = new List<Mannschaften>();
                ModelState.AddModelError("", "Fehler beim Laden der Mannschaften: " + ex.Message);
            }

            return View();
        }

        // POST: Mitglieder/Create (In Datenbank speichern)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Mitglieder neuesMitglied)
        {
            // Wir ignorieren die Navigationseigenschaften, da das Formular 
            // nur die IDs (Zahlen) und keine echten C#-Objekte senden kann.
            ModelState.Remove("Mannschaft");
            ModelState.Remove("Mannschaften");
            ModelState.Remove("Mitgliedsbeitraeges");
            ModelState.Remove("Mitgliedsbeitraege");

            if (ModelState.IsValid)
            {
                // Wenn im Dropdown "Keine Mannschaft" (Wert 0) gewählt wurde, in der DB als NULL speichern
                if (neuesMitglied.MannschaftsId == 0)
                {
                    neuesMitglied.MannschaftsId = null;
                }

                _context.Mitglieder.Add(neuesMitglied);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            // Falls die Eingabe ungültig war, laden wir das Dropdown neu für den zweiten Versuch
            ViewBag.Mannschaften = _context.Mannschaften?.ToList() ?? new List<Mannschaften>();
            return View(neuesMitglied);
        }

        // ==========================================
        // 3. UPDATE (Spieler bearbeiten)
        // ==========================================

        // GET: Mitglieder/Edit/5 (Formular mit bestehenden Daten laden)
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var mitglied = _context.Mitglieder.Find(id);
            if (mitglied == null) return NotFound();

            ViewBag.Mannschaften = _context.Mannschaften?.ToList() ?? new List<Mannschaften>();
            return View(mitglied);
        }

        // POST: Mitglieder/Edit/5 (Änderungen speichern)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Mitglieder geaendertesMitglied)
        {
            // KORREKTUR: "MitgliederId" statt "Mitglieder_Id" (Passend zu EF Core Standard)
            if (id != geaendertesMitglied.Mitglieder_Id) return NotFound();

            ModelState.Remove("Mannschaft");
            ModelState.Remove("Mannschaften");
            ModelState.Remove("Mitgliedsbeitraeges");
            ModelState.Remove("Mitgliedsbeitraege");

            if (ModelState.IsValid)
            {
                if (geaendertesMitglied.MannschaftsId == 0)
                {
                    geaendertesMitglied.MannschaftsId = null;
                }

                _context.Update(geaendertesMitglied);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Mannschaften = _context.Mannschaften?.ToList() ?? new List<Mannschaften>();
            return View(geaendertesMitglied);
        }

        // ==========================================
        // 4. DELETE (Spieler löschen)
        // ==========================================

        // GET: Mitglieder/Delete/5 (Sicherheitsabfrage anzeigen)
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            // KORREKTUR: "MitgliederId" im Lambda-Ausdruck verwendet
            var mitglied = _context.Mitglieder
                                   .Include(m => m.Mannschaft)
                                   .FirstOrDefault(m => m.Mitglieder_Id == id);

            if (mitglied == null) return NotFound();

            return View(mitglied);
        }

        // POST: Mitglieder/Delete/5 (Endgültig aus DB löschen)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var mitglied = _context.Mitglieder.Find(id);
            if (mitglied != null)
            {
                _context.Mitglieder.Remove(mitglied);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Mitglieder/Kader
        public IActionResult Kader(int? mannschaftId)
        {
            // 1. Alle Mannschaften für das Auswahl-Dropdown laden
            ViewBag.Mannschaften = _context.Mannschaften.ToList();

            // 2. Wenn eine Mannschaft ausgewählt wurde, filtere die Spieler
            if (mannschaftId.HasValue && mannschaftId.Value > 0)
            {
                var kader = _context.Mitglieder
                                    .Where(m => m.MannschaftsId == mannschaftId)
                                    .ToList();

                ViewBag.AktuelleMannschaft = _context.Mannschaften.Find(mannschaftId)?.Name;
                return View(kader);
            }

            // Wenn noch kein Team gewählt wurde, zeigen wir eine leere Liste
            return View(new List<Mitglieder>());
        }
    }
}