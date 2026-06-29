using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vereinsverwaltung.Models;

namespace Vereinsverwaltung.Controllers
{
    public class TrainerController : Controller
    {
        
        private readonly VereinsverwaltungContext _context;

        public TrainerController(VereinsverwaltungContext context)
        {
            _context = context;
        }

        // 1. ÜBERSICHT
        public async Task<IActionResult> Index()
        {
           
            var trainerListe = await _context.Trainer.ToListAsync();
            return View(trainerListe);
        }

        // 2. ERSTELLEN (Anzeige)
        public IActionResult Create()
        {
            return View();
        }

        // 2. ERSTELLEN (Speichern)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Vorname,Nachname,Telefonnummer,Lizenzstufe")] Trainer trainer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trainer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }

        // 3. BEARBEITEN (Anzeige)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var trainer = await _context.Trainer.FindAsync(id);
            if (trainer == null) return NotFound();

            return View(trainer);
        }

        // 3. BEARBEITEN (Speichern)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TrainerId,Vorname,Nachname,Telefonnummer,Lizenzstufe")] Trainer trainer)
        {
            if (id != trainer.TrainerId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trainer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Trainer.Any(e => e.TrainerId == trainer.TrainerId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }

        // 4. LÖSCHEN (Bestätigungsseite)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var trainer = await _context.Trainer.FirstOrDefaultAsync(m => m.TrainerId == id);
            if (trainer == null) return NotFound();

            return View(trainer);
        }

        // 4. LÖSCHEN (Bestätigt)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainer = await _context.Trainer.FindAsync(id);
            if (trainer != null)
            {
                _context.Trainer.Remove(trainer);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}