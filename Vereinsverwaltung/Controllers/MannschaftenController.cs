using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Vereinsverwaltung.Models;

namespace Vereinsverwaltung.Controllers
{
    public class MannschaftenController : Controller
    {
        private readonly VereinsverwaltungContext _context;

        public MannschaftenController(VereinsverwaltungContext context)
        {
            _context = context;
        }

        // 1. MANNSCHAFTS-DETAILS & KADERPLANUNG
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

           
            var mannschaft = await _context.Mannschaften
                .Include(m => m.Trainers) 
                .FirstOrDefaultAsync(m => m.MannschaftsId == id);

            if (mannschaft == null) return NotFound();

            
            var kader = await _context.Mitglieder
                .Where(m => m.MannschaftsId == id)
                .ToListAsync();


            var verfuegbareMitglieder = await _context.Mitglieder
                .Where(m => m.MannschaftsId != id)
                .Select(m => new {
                    Id = m.Mitglieder_Id,
                    Name = m.Vorname + " " + m.Nachname + " (" + (m.Status ?? "Kein Status") + ")"
                })
                .ToListAsync();

            ViewBag.Kader = kader;
            ViewBag.VerfuegbareMitglieder = new SelectList(verfuegbareMitglieder, "Id", "Name");

            return View(mannschaft);
        }

        // 2. SPIELER ZUM KADER HINZUFÜGEN (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SpielerHinzufuegen(int mannschaftId, int mitgliedId)
        {
            var mitglied = await _context.Mitglieder.FindAsync(mitgliedId);
            if (mitglied != null)
            {
                // Weist dem Mitglied die Mannschaft zu
                mitglied.MannschaftsId = mannschaftId;
                _context.Update(mitglied);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { id = mannschaftId });
        }

        // 3. SPIELER AUS DEM KADER ENTFERNEN (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SpielerEntfernen(int mannschaftId, int mitgliedId)
        {
            var mitglied = await _context.Mitglieder.FindAsync(mitgliedId);
            if (mitglied != null)
            {
                
                mitglied.MannschaftsId = null;
                _context.Update(mitglied);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { id = mannschaftId });
        }


    }
}