using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HospitalDef.Models;

namespace HospitalDef.Controllers
{
    public class CitasController : Controller
    {
        private readonly HospitalContext _context;

        public CitasController(HospitalContext context)
        {
            _context = context;
        }

        // GET: Citas
        public async Task<IActionResult> Index()
        {
            var hospitalContext = _context.Citas.Include(c => c.IdDoctorNavigation).Include(c => c.IdPacienteNavigation);
            return View(await hospitalContext.ToListAsync());
        }

        // GET: Citas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cita = await _context.Citas
                .Include(c => c.IdDoctorNavigation)
                .Include(c => c.IdPacienteNavigation)
                .FirstOrDefaultAsync(m => m.FolioCitas == id);
            if (cita == null)
            {
                return NotFound();
            }

            return View(cita);
        }

        // GET: Citas/Create
        public IActionResult Create()
        {
            var especialidades = _context.Especialidades;
            ViewData["IdDoctor"] = new SelectList(_context.Doctors, "IdDoctor", "IdDoctor");
            ViewData["IdPaciente"] = new SelectList(_context.Pacientes, "IdPaciente", "IdPaciente");
            ViewBag.Especialidades = new SelectList(especialidades, "IdEspecialidad", "Especialidades");  
            
            return View();
        }


        [HttpGet]
        public JsonResult GetDoctoresPorEspecialidad(int IdEspecialidad)
        {
            var doctoresFiltrados = _context.Doctors
                                            .Where(d => d.IdEspecialidad == IdEspecialidad)
                                            .Include(d => d.IdEmpleadoNavigation)
                                            .Select(d => new SelectListItem
                                            {
                                                Value = d.IdDoctor.ToString(),
                                                Text =(d.IdEmpleadoNavigation.Nombre + " " + d.IdEmpleadoNavigation.ApellidoP)
                                            })
                                            .ToList();


            return Json(doctoresFiltrados);
        }

        // POST: Citas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FolioCitas,idPaciente,idDoctor,fechaCita,horaCita,horaInicio,horaTermino,estatusAtencion")] Cita cita)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cita);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdDoctor"] = new SelectList(_context.Doctors, "IdDoctor", "IdDoctor", cita.idDoctor);
            ViewData["IdPaciente"] = new SelectList(_context.Pacientes, "IdPaciente", "IdPaciente", cita.idPaciente);
            DateTime ahora = DateTime.Now;
            TimeSpan anticipacionMinima = TimeSpan.FromHours(48);
            DateTime fechaMaxima = ahora.AddMonths(3);

            DateTime inicioCita = cita.fechaCita.Date.Add(cita.horaInicio.ToTimeSpan());
            DateTime finCita = cita.fechaCita.Date.Add(cita.horaTermino.ToTimeSpan());


            // --- VALIDACIÓN DE FECHA PASADA Y ANTICIPACIÓN ---
            if (inicioCita < ahora)
            {
                ModelState.AddModelError("FechaCita", "No se puede solicitar una cita con fecha y hora pasadas.");
            }


            if (inicioCita - ahora < anticipacionMinima)
            {
                ModelState.AddModelError("FechaCita", "La cita debe agendarse con al menos 48 horas de anticipación.");
            }

            if (inicioCita.Date > fechaMaxima.Date)
            {
                ModelState.AddModelError("FechaCita", "Las citas solo pueden agendarse con un máximo de 3 meses.");
            }

            if (!ModelState.IsValid)
            {
                return View(cita);
            }


            string[] estatusActivos = new[] {
                    "Agendada pendiente de pago",
                    "Pagada pendiente por atender"
                };

            var citaPendiente = await _context.Citas
                .Where(c => c.idPaciente == cita.idPaciente &&
                            c.idDoctor == cita.idDoctor &&
                            estatusActivos.Contains(c.estatusAtencion)
                )
                .FirstOrDefaultAsync();

            if (citaPendiente != null)
            {
                ModelState.AddModelError("IdPaciente", "Ya tienes una cita pendiente (agendada o pagada) con este doctor. ¡Termina esa primero!");
            }

            if (!ModelState.IsValid)
            {
                return View(cita);
            }


            // para ver si el doctor chambea en el horario que se agarro

            var doctor = await _context.Empleados.FindAsync(cita.idDoctor);
            int doctorHorario = (int) doctor.IdHorario;

            TimeSpan horaInicioRequerida = inicioCita.TimeOfDay;
            DayOfWeek diaSemana = inicioCita.DayOfWeek;
            bool horarioValido = false; // Bandera

            //horario de la mañana
            if (doctorHorario == 1)
            {
                TimeSpan inicioTurno = new TimeSpan(8, 0, 0);  // 08:00
                TimeSpan finTurno = new TimeSpan(16, 0, 0);   // 16:00

                if (diaSemana >= DayOfWeek.Monday && diaSemana <= DayOfWeek.Friday &&
                    horaInicioRequerida >= inicioTurno && horaInicioRequerida < finTurno)
                {
                    horarioValido = true;
                }
            }
            else if (doctorHorario == 2)
            {
                TimeSpan inicioTurno = new TimeSpan(16, 0, 0); // 16:00
                TimeSpan finTurno = new TimeSpan(22, 0, 0);   // 22:00

                // Debe ser un día entre Lunes y Viernes, Y la hora debe estar dentro del rango
                if (diaSemana >= DayOfWeek.Monday && diaSemana <= DayOfWeek.Friday &&
                    horaInicioRequerida >= inicioTurno && horaInicioRequerida < finTurno)
                {
                    horarioValido = true;
                }
            }
            else if (doctorHorario == 3)
            {
                TimeSpan inicioTurno = new TimeSpan(10, 0, 0);  // 10:00
                TimeSpan finTurno = new TimeSpan(17, 0, 0);   // 17:00
                if ((diaSemana == DayOfWeek.Saturday || diaSemana == DayOfWeek.Sunday) &&
                    horaInicioRequerida >= inicioTurno && horaInicioRequerida < finTurno)
                {
                    horarioValido = true;
                }
            }

            if (!horarioValido)
            {
                ModelState.AddModelError("HoraCita", $"El horario seleccionado no es válido. El doctor trabaja en el turno '{doctorHorario}' (ej: Mañana es de 9:00 a 14:00 de Lunes a Viernes).");
            }

            if (!ModelState.IsValid)
            {
                return View(cita);
            }


            //para que no se encimen las citas
            var citaEncimada = await _context.Citas
        .Where(otraCita => otraCita.idDoctor == cita.idDoctor &&
                    (otraCita.estatusAtencion == "Agendada pendiente de pago" || otraCita.estatusAtencion == "Pagada pendiente por atender") &&

                    inicioCita < otraCita.fechaCita.Date.Add(otraCita.horaTermino.ToTimeSpan()) &&
                    finCita > otraCita.fechaCita.Date.Add(otraCita.horaInicio.ToTimeSpan()))
        .FirstOrDefaultAsync();

            if (citaEncimada != null)
            {
                ModelState.AddModelError("HoraCita", "El doctor ya tiene una cita agendada o pendiente de pago que se solapa con el horario seleccionado. Intenta otra hora.");
            }
            if (!ModelState.IsValid)
            {
                return View(cita);
            }

            cita.estatusAtencion = "Agendada pendiente de pago";
            cita.fechaCreacionCita = DateTime.Now.Date;

            _context.Add(cita);
            await _context.SaveChangesAsync();

            //Ya para este punto la cita ya esta creada aqui podriamos mandarlo a otra pagina que diga no se
            // tu cita fue agendad exitosamente


            return View(cita);
        }       








            // GET: Citas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cita = await _context.Citas.FindAsync(id);
            if (cita == null)
            {
                return NotFound();
            }
            ViewData["IdDoctor"] = new SelectList(_context.Doctors, "IdDoctor", "IdDoctor", cita.idDoctor);
            ViewData["IdPaciente"] = new SelectList(_context.Pacientes, "IdPaciente", "IdPaciente", cita.idPaciente);
            return View(cita);
        }

        // POST: Citas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FolioCitas,IdPaciente,IdDoctor,FechaCreacionCita,FechaCita,HoraCita,HoraInicio,HoraTermino,EstatusPago,EstatusAtencion,LineaPago")] Cita cita)
        {
            if (id != cita.FolioCitas)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cita);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CitaExists(cita.FolioCitas))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdDoctor"] = new SelectList(_context.Doctors, "IdDoctor", "IdDoctor", cita.idDoctor);
            ViewData["IdPaciente"] = new SelectList(_context.Pacientes, "IdPaciente", "IdPaciente", cita.idPaciente);
            return View(cita);
        }

        // GET: Citas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cita = await _context.Citas
                .Include(c => c.IdDoctorNavigation)
                .Include(c => c.IdPacienteNavigation)
                .FirstOrDefaultAsync(m => m.FolioCitas == id);
            if (cita == null)
            {
                return NotFound();
            }

            return View(cita);
        }

        // POST: Citas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cita = await _context.Citas.FindAsync(id);
            if (cita != null)
            {
                _context.Citas.Remove(cita);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CitaExists(int id)
        {
            return _context.Citas.Any(e => e.FolioCitas == id);
        }
    }
}
