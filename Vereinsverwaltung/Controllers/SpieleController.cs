using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vereinsverwaltung.Models;
using System.Linq;

namespace Vereinsverwaltung.Controllers
{
    public class SpieleController : Controller
    {
        private readonly VereinsverwaltungContext _context;

        public SpieleController(VereinsverwaltungContext context)
        {
            _context = context;
        }

       
        public IActionResult Index()
        {

            var spiele = _context.Spiele
                                 .Include(s => s.Mannschafts)
                                 .Include(s => s.Platz)
                                 .OrderBy(s => s.Datum)
                                 .ThenBy(s => s.Uhrzeit)
                                 .ToList();
            return View(spiele);
        }

        
        public IActionResult Create()
        {
            ViewBag.Mannschaften = _context.Mannschaften?.ToList() ?? new List<Mannschaften>();
            ViewBag.Plaetze = _context.Plaetze?.ToList() ?? new List<Plaetze>();
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Spiele spiel)
        {
           
            ModelState.Remove("Mannschafts");
            ModelState.Remove("Platz");

            if (spiel.PlatzId == 0) spiel.PlatzId = null;

            if (ModelState.IsValid)
            {
                _context.Spiele.Add(spiel);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Mannschaften = _context.Mannschaften?.ToList() ?? new List<Mannschaften>();
            ViewBag.Plaetze = _context.Plaetze?.ToList() ?? new List<Plaetze>();
            return View(spiel);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GeneriereErgebnisse()
        {
            
            var heute = DateOnly.FromDateTime(DateTime.Today);

           
            var alteSpiele = await _context.Spiele
                .Where(s => s.Datum < heute && (s.Ergebnis == null || s.Ergebnis == ""))
                .ToListAsync();

            if (alteSpiele.Any())
            {
                Random rnd = new Random();

                foreach (var spiel in alteSpiele)
                {
                    
                    int toreHeim = rnd.Next(0, 6);
                    int toreGast = rnd.Next(0, 6);

                   
                    spiel.Ergebnis = $"{toreHeim}:{toreGast}";

                    _context.Update(spiel);
                }

                
                await _context.SaveChangesAsync();
            }

           
            return RedirectToAction(nameof(Index));
        }

    }
}