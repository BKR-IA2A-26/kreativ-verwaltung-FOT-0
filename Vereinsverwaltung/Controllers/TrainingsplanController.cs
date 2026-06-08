using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Vereinsverwaltung.Models;

namespace Vereinsverwaltung.Controllers
{
    public class TrainingsplanController : Controller
    {
        private readonly VereinsverwaltungContext _context;

        public TrainingsplanController(VereinsverwaltungContext context)
        {
            _context = context;
        }

        // GET: Trainingsplan
        public IActionResult Index()
        {
            var plan = _context.Trainingszeiten
                .Include(t => t.Mannschafts)
                .Include(t => t.Platz)
                .OrderBy(t => t.Wochentag) // Kann man später optimieren
                .ToList();
            return View(plan);
        }

        // GET: Trainingsplan/Create
        public IActionResult Create()
        {
            ViewBag.Mannschaften = new SelectList(_context.Mannschaften, "MannschaftsId", "Name");
            ViewBag.Plaetze = new SelectList(_context.Plaetze, "PlatzId", "Name");
            ViewBag.Wochentage = new SelectList(new[] { "Montag", "Dienstag", "Mittwoch", "Donnerstag", "Freitag", "Samstag", "Sonntag" });
            return View();
        }

        // POST: Trainingsplan/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Trainingszeiten training)
        {
            if (ModelState.IsValid)
            {
                _context.Add(training);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(training);
        }
    }
}