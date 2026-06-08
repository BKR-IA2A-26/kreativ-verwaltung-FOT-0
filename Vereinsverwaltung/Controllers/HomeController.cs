using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using Vereinsverwaltung.Models;

namespace Vereinsverwaltung.Controllers
{
    public class HomeController : Controller
    {
        private readonly VereinsverwaltungContext _context;

        public HomeController(VereinsverwaltungContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // 1. Statistiken abfragen
            ViewBag.MitgliederAnzahl = _context.Mitglieder.Count();
            ViewBag.MannschaftenAnzahl = _context.Mannschaften.Count();

            // 2. Das allernächste Spiel finden (heute oder in der Zukunft)
            var heute = DateOnly.FromDateTime(DateTime.Today);
            ViewBag.NaechstesSpiel = _context.Spiele
                .Include(s => s.Mannschafts)
                .Include(s => s.Platz)
                .Where(s => s.Datum >= heute)
                .OrderBy(s => s.Datum)
                .ThenBy(s => s.Uhrzeit)
                .FirstOrDefault();

            // 3. Aktuell gesperrte Plätze finden
            ViewBag.GesperrtePlaetze = _context.Plaetze
                .Where(p => p.Status != "Freigegeben")
                .ToList();

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}