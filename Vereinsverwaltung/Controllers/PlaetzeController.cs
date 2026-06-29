using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Vereinsverwaltung.Models;

namespace Vereinsverwaltung.Controllers
{
    public class PlaetzeController : Controller
    {
        private readonly VereinsverwaltungContext _context;

        public PlaetzeController(VereinsverwaltungContext context)
        {
            _context = context;
        }

      
        public IActionResult Index()
        {
         
            var plaetze = _context.Plaetze.Include(p => p.Spiele).ToList();

           
            string heuteWochentag = DateTime.Today.ToString("dddd", new System.Globalization.CultureInfo("de-DE"));
            ViewBag.AktuellerWochentag = heuteWochentag;

            
            ViewBag.HeutigeTrainings = _context.Trainingszeiten
                .Include(t => t.Mannschafts)
                .Where(t => t.Wochentag == heuteWochentag)
                .ToList();

            return View(plaetze);
        }

        
        public IActionResult Edit(int id)
        {
            var platz = _context.Plaetze.Find(id);
            if (platz == null) return NotFound();
            return View(platz);
        }

    
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Plaetze platz)
        {
            if (id != platz.PlatzId) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(platz);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(platz);
        }
    }
}