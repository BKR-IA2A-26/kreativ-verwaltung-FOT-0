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
            
            ViewBag.MitgliederAnzahl = _context.Mitglieder.Count();
            ViewBag.MannschaftenAnzahl = _context.Mannschaften.Count();

            
            var heute = DateOnly.FromDateTime(DateTime.Today);
            ViewBag.NaechstesSpiel = _context.Spiele
                .Include(s => s.Mannschafts)
                .Include(s => s.Platz)
                .Where(s => s.Datum >= heute)
                .OrderBy(s => s.Datum)
                .ThenBy(s => s.Uhrzeit)
                .FirstOrDefault();

            //  Aktuell gesperrte Plätze anzeigen
            ViewBag.GesperrtePlaetze = _context.Plaetze
                .Where(p => p.Status != "Freigegeben")
                .ToList();


            //  Mitglieder aus der Datenbank holen
            var alleMitglieder = _context.Mitglieder.ToList();

            int gesamt = alleMitglieder.Count;
            int bezahlt = alleMitglieder.Count(m => m.HatBezahlt);

            
            double prozentBezahlt = gesamt > 0 ? Math.Round((double)bezahlt / gesamt * 100) : 0;


            ViewBag.MitgliederAnzahl = gesamt;
            ViewBag.BezahltProzent = prozentBezahlt;
            ViewBag.OffeneBeitraege = gesamt - bezahlt;

           
            string ampelFarbe = "bg-danger"; 
            if (prozentBezahlt >= 85)
            {
                ampelFarbe = "bg-success";
            }
            else if (prozentBezahlt >= 50)
            {
                ampelFarbe = "bg-warning text-dark"; 
            }
            ViewBag.AmpelFarbe = ampelFarbe;




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