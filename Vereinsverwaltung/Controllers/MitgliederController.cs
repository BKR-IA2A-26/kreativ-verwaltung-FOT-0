using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vereinsverwaltung.Models;
using System.Linq;

namespace Vereinsverwaltung.Controllers
{
    public class MitgliederController : Controller
    {
        private readonly VereinsverwaltungContext _context;

        
        public MitgliederController(VereinsverwaltungContext context)
        {
            _context = context;
        }

        
        public IActionResult Index(string searchString, int? mannschaftId, string status)
        {
            ViewBag.Mannschaften = _context.Mannschaften?.ToList() ?? new List<Mannschaften>();

            ViewBag.CurrentSearch = searchString;
            ViewBag.CurrentMannschaft = mannschaftId;
            ViewBag.CurrentStatus = status;

            var spielerQuery = _context.Mitglieder.Include(m => m.Mannschaft).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                spielerQuery = spielerQuery.Where(m => m.Vorname.Contains(searchString) ||
                                                      m.Nachname.Contains(searchString));
            }

            if (mannschaftId.HasValue && mannschaftId.Value > 0)
            {
                spielerQuery = spielerQuery.Where(m => m.MannschaftsId == mannschaftId);
            }

            if (!string.IsNullOrEmpty(status))
            {
                spielerQuery = spielerQuery.Where(m => m.Status == status);
            }

            return View(spielerQuery.ToList());
        }

       
        [HttpGet]
        public IActionResult Create()
        {
            try
            {
                
                var listenDaten = _context.Mannschaften?.ToList();
                ViewBag.Mannschaften = listenDaten ?? new List<Mannschaften>();
            }
            catch (Exception ex)
            {
                
                ViewBag.Mannschaften = new List<Mannschaften>();
                ModelState.AddModelError("", "Fehler beim Laden der Mannschaften: " + ex.Message);
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Mitglieder neuesMitglied)
        {
           
            ModelState.Remove("Mannschaft");
            ModelState.Remove("Mannschaften");
            ModelState.Remove("Mitgliedsbeitraeges");
            ModelState.Remove("Mitgliedsbeitraege");

            if (ModelState.IsValid)
            {
                if (neuesMitglied.MannschaftsId == 0)
                {
                    neuesMitglied.MannschaftsId = null;
                }

                _context.Mitglieder.Add(neuesMitglied);
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Mannschaften = _context.Mannschaften?.ToList() ?? new List<Mannschaften>();
            return View(neuesMitglied);
        }

        

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null) return NotFound();

            var mitglied = _context.Mitglieder.Find(id);
            if (mitglied == null) return NotFound();

            ViewBag.Mannschaften = _context.Mannschaften?.ToList() ?? new List<Mannschaften>();
            return View(mitglied);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Mitglieder geaendertesMitglied)
        {
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

        
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null) return NotFound();

            var mitglied = _context.Mitglieder
                                   .Include(m => m.Mannschaft)
                                   .FirstOrDefault(m => m.Mitglieder_Id == id);

            if (mitglied == null) return NotFound();

            return View(mitglied);
        }

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

        public IActionResult Kader(int? mannschaftId)
        {
            ViewBag.Mannschaften = _context.Mannschaften.ToList();

            if (mannschaftId.HasValue && mannschaftId.Value > 0)
            {
                var kader = _context.Mitglieder
                                    .Where(m => m.MannschaftsId == mannschaftId)
                                    .ToList();

                ViewBag.AktuelleMannschaft = _context.Mannschaften.Find(mannschaftId)?.Name;
                return View(kader);
            }

            return View(new List<Mitglieder>());
        }
    }
}