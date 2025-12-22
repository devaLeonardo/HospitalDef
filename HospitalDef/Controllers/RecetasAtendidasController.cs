using HospitalDef.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalDef.Controllers
{
    public class RecetasAtendidasController : Controller
    {
        private readonly HospitalContext _context;

        public RecetasAtendidasController(HospitalContext context)
        {
            _context = context;
        }

        // Listado general o filtrado por doctor
        public async Task<IActionResult> Index()
        {
            var recetas = await _context.RecetasAtendidas.ToListAsync();
            return View(recetas);
        }
    }
}
