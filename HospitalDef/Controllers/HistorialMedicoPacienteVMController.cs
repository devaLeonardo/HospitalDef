using HospitalDef.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalDef.Controllers
{
    public class HistorialMedicoPacienteVMController : Controller
    {
        private readonly HospitalContext _context;

        public HistorialMedicoPacienteVMController(HospitalContext context)
        {
            _context = context;
        }

        // Carga TODO el historial permitido
        public IActionResult Index()
        {
            var historial = _context.HistorialMedicoPacienteParaDoctor
                .AsNoTracking()
                .OrderByDescending(h => h.FechaMovimiento)
                .ToList();

            return View(historial);
        }

        // GET: HistorialMedicoPacienteVMController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: HistorialMedicoPacienteVMController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: HistorialMedicoPacienteVMController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: HistorialMedicoPacienteVMController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: HistorialMedicoPacienteVMController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: HistorialMedicoPacienteVMController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: HistorialMedicoPacienteVMController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
