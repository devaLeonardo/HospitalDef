using HospitalDef.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HospitalDef.Controllers
{
    public class vistaDoctoresController : Controller
    {
        private readonly HospitalContext _context;

        public vistaDoctoresController(HospitalContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var doctores = await _context.VistaDoctores.ToListAsync();
            return View(doctores);
        }
    }
}
