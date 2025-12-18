using HospitalDef.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Claims;
using System.Threading.Tasks;

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
            if (!User.Identity.IsAuthenticated)
                return View(new List<Cita>());

            // ID del usuario logueado
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var role = User.FindFirst(ClaimTypes.Role).Value;

            
            var citas = new List<Cita>();

            



            if (role.Equals("Doctor"))
            {
                var doctor = await _context.Doctors
                .Include(d => d.IdEmpleadoNavigation)
                .FirstOrDefaultAsync(d => d.IdEmpleadoNavigation.IdUsuario == userId);

                if(doctor == null)
                    return BadRequest("No se encontró el doctor del usuario logueado.");

                var citasDoc = await _context.Citas
                       .Where(c => c.idDoctor == doctor.IdDoctor)
                       .Include(c=>c.IdPacienteNavigation)
                       .Include(c => c.IdDoctorNavigation)
                           .ThenInclude(d => d.IdEmpleadoNavigation)
                       .Include(c => c.IdDoctorNavigation.IdEspecialidadNavigation)
                           .Include(c => c.IdDoctorNavigation)
                           .ThenInclude(d => d.IdEmpleadoNavigation)
                           .Include(c => c.IdDoctorNavigation)
                           .ThenInclude(d => d.IdEspecialidadNavigation)
                           .Include(c => c.IdDoctorNavigation)
                           .ThenInclude(d => d.IdConsultorioNavigation)
                           .ToListAsync();
                citas = citasDoc;

            }
            if(role.Equals("Paciente"))
            {
                var paciente = await _context.Pacientes
                .FirstOrDefaultAsync(p => p.IdUsuario == userId);

                if(paciente == null)
                    return BadRequest("No se encontró el paciente del usuario logueado.");

                var citasPac = await _context.Citas
                       .Where(c => c.idPaciente == paciente.IdPaciente)
                       .Include(c => c.IdDoctorNavigation)
                           .ThenInclude(d => d.IdEmpleadoNavigation)
                       .Include(c => c.IdDoctorNavigation.IdEspecialidadNavigation)
                           .Include(c => c.IdDoctorNavigation)
                           .ThenInclude(d => d.IdEmpleadoNavigation)
                           .Include(c => c.IdDoctorNavigation)
                           .ThenInclude(d => d.IdEspecialidadNavigation)
                           .Include(c => c.IdDoctorNavigation)
                           .ThenInclude(d => d.IdConsultorioNavigation)
                           .ToListAsync();

                citas = citasPac;
            }
                
            bool cambios = false;


            if (citas == null)
            {
                return NotFound("No se encontraron citas para este paciente.");
            }
            else
            {

                foreach (var citaEspecifica in citas)
                {
                    if (citaEspecifica.estatusAtencion.Contains("Agendada pendiente de pago") &&
                        citaEspecifica.fechaCreacionCita.AddHours(8) < DateTime.Now)
                    {
                        citaEspecifica.estatusAtencion = "Cancelada por falta de pago";
                        cambios = true;
                    }
                    var citaDateTime = citaEspecifica.fechaCita + citaEspecifica.horaCita;  
                    if (citaEspecifica.estatusAtencion.Contains("Cita pagada pendiente por atender") &&
                        citaDateTime < DateTime.Now)
                    {
                        citaEspecifica.estatusAtencion = "Atendida";
                        cambios = true;
                    }
                }

                if (cambios)
                    await _context.SaveChangesAsync();
            }


            if(!role.Equals("Paciente"))
                return View("IndexDoctor", citas);
            else
                return View("Index", citas);

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
                .Include(c=>c.Receta)
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


            ViewData["IdDoctor"] = new SelectList(_context.Doctors, "IdDoctor", "IdDoctor");
            // Id del usuario loggeado
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
                return RedirectToAction("Login", "Acceso");

            // Buscar al paciente relacionado con este usuario
            var paciente = _context.Pacientes.FirstOrDefault(p => p.IdUsuario == int.Parse(userId));

            if (paciente == null)
                return BadRequest("No se encontró el paciente del usuario logueado.");

            // pasar el paciente fijo
            ViewBag.PacienteNombre = paciente.Nombre;
            ViewBag.PacienteId = paciente.IdPaciente;

            var especialidades = _context.Especialidades;
            ViewBag.Especialidades = new SelectList(especialidades, "IdEspecialidad", "Especialidades");

            var cita = new Cita
            {
                fechaCita = DateTime.Now//le pasa la fecha y hora del dia de hoy al calendario
            };
            if (cita.idDoctor.HasValue)
                CargarHorarioDoctor(cita.idDoctor.Value);


            return View(cita);
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
                                                Text = (d.IdEmpleadoNavigation.Nombre + " " + d.IdEmpleadoNavigation.ApellidoP)
                                            })
                                            .ToList();


            return Json(doctoresFiltrados);
        }

        // POST: Citas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Cita cita)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var paciente = await _context.Pacientes.FirstOrDefaultAsync(p => p.IdUsuario == int.Parse(userId));

            if (paciente == null)
                return BadRequest("No se encontró el paciente del usuario logeado.");

            // Fijar SIEMPRE el paciente
            cita.idPaciente = paciente.IdPaciente;

            // Si la fecha viene sin valor (cuando falla validación)
            if (cita.fechaCita == default)
                cita.fechaCita = DateTime.Today;

            cita.fechaCreacionCita = DateTime.Now;
            cita.estatusAtencion = "Agendada pendiente de pago";
            cita.horaCita = new TimeSpan(cita.fechaCita.Hour, cita.fechaCita.Minute, 0);
            cita.horaInicio = cita.horaCita;
            cita.horaTermino = cita.horaInicio.Add(TimeSpan.FromMinutes(60));

            ModelState.Clear();
            TryValidateModel(cita);

            DateTime inicioCita = cita.fechaCita;
            DateTime finCita = inicioCita.AddMinutes(60);

            DateTime ahora = DateTime.Now;
            TimeSpan anticipacionMinima = TimeSpan.FromHours(48);
            DateTime fechaMaxima = ahora.AddMonths(3);

            // --- VALIDACIONES ----------------------

            if (inicioCita < ahora)
                ModelState.AddModelError("FechaCita", "No se puede solicitar una cita con fecha y hora pasadas.");

            if (inicioCita.Date == ahora.Date)
                ModelState.AddModelError("FechaCita", "No se puede solicitar una cita de hoy para hoy.");

            if (inicioCita - ahora < anticipacionMinima)
                ModelState.AddModelError("FechaCita", "La cita debe agendarse con al menos 48 horas de anticipación.");

            if (inicioCita.Date > fechaMaxima.Date)
                ModelState.AddModelError("FechaCita", "Las citas solo pueden agendarse con un máximo de 3 meses.");

            // SI FALLA, recargar combos y paciente
            if (!ModelState.IsValid)
            {
                CargarCombos(cita);
                if (cita.idDoctor.HasValue && cita.idDoctor.Value != 0)
                    CargarHorarioDoctor(cita.idDoctor.Value);//no separace el horario
                return View(cita);
            }

            // Validar citas pendientes
            string[] estatusActivos = {
                "Agendada pendiente de pago",
                "Pagada pendiente por atender"
            };

            var citaPendiente = await _context.Citas
                .Where(c => c.idPaciente == cita.idPaciente &&
                            c.idDoctor == cita.idDoctor &&
                            estatusActivos.Contains(c.estatusAtencion))
                .FirstOrDefaultAsync();

            if (citaPendiente != null)
                ModelState.AddModelError("FechaCita", "Ya tienes una cita pendiente con este doctor.");

            if (!ModelState.IsValid)
            {
                CargarCombos(cita);
                if (cita.idDoctor.HasValue && cita.idDoctor.Value != 0)
                    CargarHorarioDoctor(cita.idDoctor.Value);//no separace el horario
                return View(cita);
            }

            // Validar horario del doctor
            var doctor = await _context.Doctors.FindAsync(cita.idDoctor);
            var doctorBien = await _context.Empleados.FindAsync(doctor.IdEmpleado);
            int doctorHorario = (int)doctorBien.IdHorario;

            TimeSpan horaInicioRequerida = inicioCita.TimeOfDay;
            DayOfWeek diaSemana = inicioCita.DayOfWeek;
            bool horarioValido = false;

            if (doctorHorario == 1)
            {
                TimeSpan inicioTurno = new TimeSpan(8, 0, 0);
                TimeSpan finTurno = new TimeSpan(16, 0, 0);

                if (diaSemana >= DayOfWeek.Monday && diaSemana <= DayOfWeek.Friday &&
                    horaInicioRequerida >= inicioTurno && horaInicioRequerida < finTurno)
                    horarioValido = true;
            }
            else if (doctorHorario == 2)
            {
                TimeSpan inicioTurno = new TimeSpan(16, 0, 0);
                TimeSpan finTurno = new TimeSpan(22, 0, 0);

                if (diaSemana >= DayOfWeek.Monday && diaSemana <= DayOfWeek.Friday &&
                    horaInicioRequerida >= inicioTurno && horaInicioRequerida < finTurno)
                    horarioValido = true;
            }
            else if (doctorHorario == 3)
            {
                TimeSpan inicioTurno = new TimeSpan(10, 0, 0);
                TimeSpan finTurno = new TimeSpan(17, 0, 0);

                if ((diaSemana == DayOfWeek.Saturday || diaSemana == DayOfWeek.Sunday) &&
                    horaInicioRequerida >= inicioTurno && horaInicioRequerida < finTurno)
                    horarioValido = true;
            }

            if (!horarioValido)
                ModelState.AddModelError("FechaCita", $"El horario seleccionado no es válido para este doctor.");

            if (!ModelState.IsValid)
            {
                CargarCombos(cita);
                if (cita.idDoctor.HasValue && cita.idDoctor.Value != 0)
                    CargarHorarioDoctor(cita.idDoctor.Value);//no separace el horario
                return View(cita);
            }

            // Validar encimado de citas
            var citasDelDoctor = await _context.Citas
                .Where(c =>
                    c.idDoctor == cita.idDoctor &&
                    (c.estatusAtencion == "Agendada pendiente de pago" ||
                     c.estatusAtencion == "Pagada pendiente por atender"))
                .ToListAsync();

            var citaEncimada = citasDelDoctor
                .FirstOrDefault(otra =>
                    inicioCita < otra.fechaCita.Add(otra.horaTermino) &&
                    finCita > otra.fechaCita.Add(otra.horaInicio));

            if (citaEncimada != null)
                ModelState.AddModelError("FechaCita", "El doctor ya tiene una cita en ese horario.");

            if (!ModelState.IsValid)
            {
                CargarCombos(cita);
                if (cita.idDoctor.HasValue && cita.idDoctor.Value != 0)
                    CargarHorarioDoctor(cita.idDoctor.Value);//no separace el horario
                return View(cita);
            }

            // Guardar en la base de datos los cambios
            _context.Add(cita);
            await _context.SaveChangesAsync();

            string mensaje = "Recuerda que solo tienes 8 horas para pagar tu cita";
            TempData["Alerta Pago"] = mensaje;

            return RedirectToAction("Index", "Citas");
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
            Cita cita = null;

            try
            {
                cita = await _context.Citas.FirstOrDefaultAsync(c => c.FolioCitas == id);

                if (cita == null)
                {
                    ModelState.AddModelError("estatusAtencion", "La cita no existe. " + id);
                    return View(new Cita());
                }

                DateTime citaCompleta = cita.fechaCita.Date + cita.horaCita;

                if (citaCompleta <= DateTime.Now)
                {
                    ModelState.AddModelError("estatusAtencion", "No se puede cancelar una cita pasada o en curso.");
                    return View(cita);
                }

                var tiempoRestante = citaCompleta - DateTime.Now;

                if (tiempoRestante >= TimeSpan.FromHours(48))
                    TempData["Alerta"] = "Cita eliminada. No se aplico ninguna penalizacion";
                else if (tiempoRestante >= TimeSpan.FromHours(24) && tiempoRestante < TimeSpan.FromHours(48))
                    TempData["Alerta"] = "Cita eliminada. Se aplico una penalizacion del 50%.";
                else
                    TempData["Alerta"] = "Cita eliminada. Se aplico una penalizacion del 100%.";

                cita.estatusAtencion = "CANCELADO";

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("estatusAtencion", ex.ToString());
                return View(cita ?? new Cita());
            }
        }


        private bool CitaExists(int id)
        {
            return _context.Citas.Any(e => e.FolioCitas == id);
        }


        //metodo para pagar citas
        [HttpPost]
        public async Task<IActionResult> PagarCita(int folio)
        {
            try
            {
                var cita = await _context.Citas.FirstOrDefaultAsync(c => c.FolioCitas == folio);

                if (cita == null)
                {

                    return Json(new { ok = false, mensaje = "La cita no existe"});
                }
                if ((!cita.estatusAtencion.Contains("pendiente") && !cita.estatusAtencion.Contains("pago")) ||
                    cita.estatusAtencion.Contains("Cancelada"))
                {

                    return Json(new { ok = false, mensaje = "La cita ya ha sido pagada previamente o ha sido cancelada" });   

                }

                cita.estatusAtencion = "PAGADO";//aqui se dispara el trigger ya que le pasa el parametro pagado

                await _context.SaveChangesAsync();

                return Json(new { ok = true, mensaje = "La cita ha sido pagada con éxito." });
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, mensaje = "ERROR SERVIDOR: " + ex.Message });
            }
        }







        //metodo para recargar las citas y se mantengan guardados
        private void CargarCombos(Cita cita)
        {
            // Especialidades
            ViewBag.Especialidades = new SelectList(
                _context.Especialidades,
                "IdEspecialidad",
                "Especialidades",
                cita.EspecialidadIdFiltro
            );

            // Doctores filtrados por especialidad
            var doctoresFiltrados = _context.Doctors
                .Where(d => d.IdEspecialidad == cita.EspecialidadIdFiltro)
                .Include(d => d.IdEmpleadoNavigation)
                .Select(d => new SelectListItem
                {
                    Value = d.IdDoctor.ToString(),
                    Text = d.IdEmpleadoNavigation.Nombre + " " + d.IdEmpleadoNavigation.ApellidoP
                })
                .ToList();

            ViewBag.Doctores = new SelectList(
                doctoresFiltrados,
                "Value",
                "Text",
                cita.idDoctor
            );

            // PACIENTE LOGGEADO
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var paciente = _context.Pacientes.FirstOrDefault(p => p.IdUsuario == int.Parse(userId));

            ViewBag.PacienteNombre = paciente?.Nombre;
            ViewBag.PacienteId = paciente?.IdPaciente;
        }
        public IActionResult GetHorarioDoctor(int idDoctor)
        {
            var horario = _context.VistaDoctorHorario
                .Where(h => h.IdDoctor == idDoctor)
                .Select(h => new
                {
                    dia = h.DiaSemana,
                    inicio = h.HoraInicio.ToString(@"hh\:mm"),
                    fin = h.HoraFin.ToString(@"hh\:mm"),
                    consultorio = h.Consultorio
                })
                .ToList();

            return Json(horario);
        }


        private void CargarHorarioDoctor(int idDoctor)
        {
            var horario = _context.VistaDoctorHorario
                .Where(h => h.IdDoctor == idDoctor)
                .ToList();

            ViewBag.HorarioDoctor = horario;
        }


    }


}
