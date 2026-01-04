using HospitalDef.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace HospitalDef.Controllers
{
    public class CitasController : Controller
    {
        private readonly HospitalContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;


        public CitasController(HospitalContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }



        public async Task<IActionResult> generarPDF(int id)
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjGyl/Vkd+XU9FcVRDQmJKYVF2R2VJelRyfF9FZ0wxOX1dQl9mSHxSckRkW3ZeeHRWQWFXUkU=");

            var citaPDF = await _context.Citas
                .Include(r => r.IdPacienteNavigation)
                .Include(r => r.IdDoctorNavigation)
                .ThenInclude(d => d.IdEspecialidadNavigation)
                 .Include(r => r.IdDoctorNavigation)
                .ThenInclude(e => e.IdEmpleadoNavigation)
                .Include(r => r.IdDoctorNavigation)
                .ThenInclude(e => e.IdConsultorioNavigation)
                .FirstOrDefaultAsync(m => m.FolioCitas == id);

            var fecha = citaPDF?.fechaCita.ToString("dd/MM/yyyy");

            DateTime? fechaCita = citaPDF?.fechaCita;
            




            PdfDocument pdfDocument = new PdfDocument();
            PdfPage currentPage = pdfDocument.Pages.Add();
            SizeF clientSize = currentPage.GetClientSize();

            //aqui se agrega la imagen

            string webRootPath = _hostEnvironment.WebRootPath; // Obtiene la ruta a wwwroot
            string rutaFisicaImagen = Path.Combine(webRootPath, "images", "logo.png");

            PdfImage icon = null;
            FileStream imageStream = null;

            try
            {
                // Verifica que la imagen exista
                if (System.IO.File.Exists(rutaFisicaImagen))
                {
                    imageStream = new FileStream(rutaFisicaImagen, FileMode.Open, FileAccess.Read);
                    icon = new PdfBitmap(imageStream);
                    // ... (Tu código de dibujo continúa)
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores: Si la imagen no carga, continúa sin ella.
                System.Diagnostics.Debug.WriteLine($"Error al cargar la imagen: {ex.Message}");
            }
            finally
            {
                // Es importante cerrar el stream después de usarlo
                imageStream?.Dispose();
            }

            // Dibuja la imagen SOLO si se cargó correctamente
            PdfGraphics graphics = currentPage.Graphics;

            SizeF iconSize = new SizeF(80, 80);
            PointF iconLocation = new PointF(14, 13);
            graphics.DrawImage(icon, iconLocation, iconSize);




            //aqui se agrega la fecha


            PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold);
            var text = new PdfTextElement(fecha, font);
            text.StringFormat = new PdfStringFormat(PdfTextAlignment.Right);
            PdfLayoutResult result = text.Draw(currentPage, new PointF(clientSize.Width - 25, iconLocation.Y + 10));


            //titulo de receta medica
            font = new PdfStandardFont(PdfFontFamily.Helvetica, 16, PdfFontStyle.Bold);
            text = new PdfTextElement("Comprobante de Cita", font);
            text.StringFormat = new PdfStringFormat(PdfTextAlignment.Right);
            result = text.Draw(currentPage, new PointF(clientSize.Width - 200, iconLocation.Y + 20));



            //aqui se agrega la direccion del hospital

            font = new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold);
            text = new PdfTextElement("Calle Dr. Olvera, 123", font, new PdfSolidBrush(Color.FromArgb(0, 0, 0, 0)));
            text.StringFormat = new PdfStringFormat(PdfTextAlignment.Right);
            result = text.Draw(currentPage, new PointF(clientSize.Width - 25, result.Bounds.Y + 10));


            font = new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold);
            text = new PdfTextElement("Col. Doctores", font, new PdfSolidBrush(Color.FromArgb(0, 0, 0, 0)));
            text.StringFormat = new PdfStringFormat(PdfTextAlignment.Right);
            result = text.Draw(currentPage, new PointF(clientSize.Width - 25, result.Bounds.Y + 10));


            font = new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold);
            text = new PdfTextElement("Ciudad De México", font, new PdfSolidBrush(Color.FromArgb(0, 0, 0, 0)));
            text.StringFormat = new PdfStringFormat(PdfTextAlignment.Right);
            result = text.Draw(currentPage, new PointF(clientSize.Width - 25, result.Bounds.Y + 10));



            //datos generales

            font = new PdfStandardFont(PdfFontFamily.Helvetica, 12);
            text = new PdfTextElement("Paciente: "+citaPDF?.IdPacienteNavigation?.NombreCompleto, font);
            result = text.Draw(currentPage, new PointF(14, result.Bounds.Bottom + 50));

            font = new PdfStandardFont(PdfFontFamily.Helvetica, 12);
            text = new PdfTextElement("Folio Cita: " + citaPDF?.FolioCitas, font);
            result = text.Draw(currentPage, new PointF(14, result.Bounds.Bottom + 10));

            font = new PdfStandardFont(PdfFontFamily.Helvetica, 12);
            text = new PdfTextElement("Estatus Cita: " + citaPDF?.estatusAtencion, font);
            result = text.Draw(currentPage, new PointF(14, result.Bounds.Bottom + 10));





            //linea divisora
            PdfGraphics graphics2 = currentPage.Graphics;
            PdfPen bluePen = new PdfPen(Color.FromArgb(53, 67, 168), 2f);
            graphics2.DrawLine(bluePen, new PointF(14, result.Bounds.Bottom + 15), new PointF(clientSize.Width - 14, result.Bounds.Bottom + 15));






            //contenido de la receta
            font = new PdfStandardFont(PdfFontFamily.Helvetica, 16);
            text = new PdfTextElement("Datos de la Cita", font);
            result = text.Draw(currentPage, new PointF(230, result.Bounds.Bottom + 20));

            font = new PdfStandardFont(PdfFontFamily.Helvetica, 10, PdfFontStyle.Bold);
            text = new PdfTextElement("- Fecha de Creación de la cita: " + citaPDF?.fechaCreacionCita.ToShortDateString() , font);
            result = text.Draw(currentPage, new PointF(14, result.Bounds.Bottom + 20));
            font = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
            text = new PdfTextElement("- Fecha de la Cita: "+citaPDF?.fechaCita.ToShortDateString() , font);
            result = text.Draw(currentPage, new PointF(14, result.Bounds.Bottom + 10));
            font = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
            text = new PdfTextElement("- Hora de inicio de la cita: " + citaPDF.horaInicio , font);
            result = text.Draw(currentPage, new PointF(14, result.Bounds.Bottom + 10));
            font = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
            text = new PdfTextElement("- Hora de termino de la cita: " + citaPDF.horaTermino, font);
            result = text.Draw(currentPage, new PointF(14, result.Bounds.Bottom + 10));
            font = new PdfStandardFont(PdfFontFamily.Helvetica, 10,PdfFontStyle.Bold);
            text = new PdfTextElement("- Doctor seleccionado: " + citaPDF.IdDoctorNavigation?.IdEmpleadoNavigation?.Nombre +" " +citaPDF.IdDoctorNavigation?.IdEmpleadoNavigation?.ApellidoP, font);
            result = text.Draw(currentPage, new PointF(14, result.Bounds.Bottom + 10));
            font = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
            text = new PdfTextElement("- Especialidad: " + citaPDF.IdDoctorNavigation?.IdEspecialidadNavigation?.Especialidades, font);
            result = text.Draw(currentPage, new PointF(14, result.Bounds.Bottom + 10));
            font = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
            text = new PdfTextElement("- Consultorio: " + citaPDF.IdDoctorNavigation.IdConsultorioNavigation.Edificio +" " +citaPDF.IdDoctorNavigation.IdConsultorioNavigation.Planta + " "+citaPDF.IdDoctorNavigation.IdConsultorioNavigation.Numero, font);
            result = text.Draw(currentPage, new PointF(14, result.Bounds.Bottom + 10));


            font = new PdfStandardFont(PdfFontFamily.Helvetica, 10,PdfFontStyle.Bold);
            text = new PdfTextElement("- Estatus: " + citaPDF.estatusAtencion, font);
            result = text.Draw(currentPage, new PointF(14, result.Bounds.Bottom + 10));


            font = new PdfStandardFont(PdfFontFamily.Helvetica, 15, PdfFontStyle.Bold);
            text = new PdfTextElement("PAGASTE LA CITA CORRECTAMENTE", font);
            result = text.Draw(currentPage, new PointF(200, result.Bounds.Bottom + 30));




            // Aqui se guarda el documento PDF en un MemoryStream
            MemoryStream stream = new MemoryStream();
            pdfDocument.Save(stream);
            pdfDocument.Close(true);
            stream.Position = 0;

            string nombreArchivo = "Comprobante Cita.pdf";

            return File(stream, "application/pdf", nombreArchivo);
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
                       .OrderByDescending(c => c.fechaCita)
                       .ThenByDescending(c => c.horaCita)
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
                       .OrderByDescending(c => c.fechaCita)
                       .ThenByDescending(c => c.horaCita)
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
            if (!User.Identity.IsAuthenticated)
                return NotFound();

            // ID del usuario logueado
            var userId = int.Parse(User?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var role = User?.FindFirst(ClaimTypes.Role)?.Value;


            if (userId == 0)
                return RedirectToAction("Login", "Acceso");


            if (role.Equals("Paciente"))
            {

                ViewData["IdDoctor"] = new SelectList(_context.Doctors, "IdDoctor", "IdDoctor");
                var paciente = _context.Pacientes.FirstOrDefault(p => p.IdUsuario == userId);

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


                return View("Create", cita);
            }
            else
            {

               

                ViewData["IdDoctor"] = new SelectList(_context.Doctors, "IdDoctor", "IdDoctor");
                
                //ViewBag.Paciente = new SelectList(_context.Pacientes, "IdPaciente", "NombreCompleto");
                ViewBag.Paciente = _context.Pacientes
                                            .Include(d => d.IdUsuarioNavigation)
                                            .Select(d => new SelectListItem
                                            {
                                                Value = d.IdPaciente.ToString(),
                                                Text = ("["+ d.IdUsuarioNavigation.NombreUsuario +"] " + d.Nombre + " " + d.ApellidoP + " " + d.ApellidoM)
                                            })
                                            .ToList();
                

                var especialidades = _context.Especialidades;
                ViewBag.Especialidades = new SelectList(especialidades, "IdEspecialidad", "Especialidades");

                var cita = new Cita
                {
                    fechaCita = DateTime.Now//le pasa la fecha y hora del dia de hoy al calendario
                };

                if (cita.idDoctor.HasValue)
                    CargarHorarioDoctor(cita.idDoctor.Value);

                return View("CreateRecepcion",cita);
            }
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
            if (!User.Identity.IsAuthenticated)
                return NotFound();

            // ID del usuario logueado
            var userId = int.Parse(User?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var role = User?.FindFirst(ClaimTypes.Role)?.Value;


            if (userId == 0)
                return RedirectToAction("Login", "Acceso");


            if (role.Equals("Paciente"))
            {
                var paciente = await _context.Pacientes.FirstOrDefaultAsync(p => p.IdUsuario == userId);

                if (paciente == null)
                    return BadRequest("No se encontró el paciente del usuario logeado.");

                // Fijar SIEMPRE el paciente
                cita.idPaciente = paciente.IdPaciente;


            }

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

                //Cargar dependiendo de quien sea
                if (role.Equals("Paciente"))
                    return View("Create",cita);
                else
                    return View("CreateRecepcion", cita);

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
                //Cargar dependiendo de quien sea
                if (role.Equals("Paciente"))
                    return View("Create", cita);
                else
                    return View("CreateRecepcion", cita);
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
                //Cargar dependiendo de quien sea
                if (role.Equals("Paciente"))
                    return View("Create", cita);
                else
                    return View("CreateRecepcion", cita);
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
                //Cargar dependiendo de quien sea
                if (role.Equals("Paciente"))
                    return View("Create", cita);
                else
                    return View("CreateRecepcion", cita);
            }

            // Guardar en la base de datos los cambios
            _context.Add(cita);
            await _context.SaveChangesAsync();



            //Cargar dependiendo de quien sea
            if (role.Equals("Paciente"))
            {
                string mensaje = "Recuerda que solo tienes 8 horas para pagar tu cita";
                TempData["Alerta Pago"] = mensaje;

                return RedirectToAction("Index", "Citas");
            }
            else
            {
                string mensaje = "Recuerdale al paciente que solo tiene 8 horas para pagar su cita";
                TempData["Alerta Pago"] = mensaje;

                return RedirectToAction("Index", "Recepcionistums");
            }



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


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AtenderCita(int folio)
        {
            try
            {
                var cita = await _context.Citas.FirstOrDefaultAsync(c => c.FolioCitas == folio);

                if (cita == null)
                {

                    return Json(new { ok = false, mensaje = "La cita no existe" });
                }


                if (cita.estatusAtencion.Contains("Atendida"))
                {

                    return Json(new { ok = false, mensaje = "La cita ya fue atendida" });
                }

                if (cita.fechaCita.Date > DateTime.Today)
                {
                   return Json(new { ok = false, mensaje = "No puedes atender una cita antes de su fecha programada." });
                }


                if ((cita.estatusAtencion.Contains("pendiente") && cita.estatusAtencion.Contains("pago")) ||
                    cita.estatusAtencion.Contains("Cancelada"))
                {

                    return Json(new { ok = false, mensaje = "La cita NO ha sido pagada previamente o ha sido cancelada" });

                }

                cita.estatusAtencion = "Atendida";

                await _context.SaveChangesAsync();

                return Json(new { ok = true, mensaje = "La cita ha sido Atendida con éxito." });
            }
            catch (Exception ex)
            {
                return Json(new { ok = false, mensaje = "ERROR SERVIDOR: " + ex.Message });
            }
        }


        [HttpGet]
        public async Task<IActionResult> Cancelar()
        {
            var citas = await _context.HistorialCitasMedicoPacientes
                .OrderByDescending(c => c.Fecha)
                .ThenByDescending(c => c.Hora)
                .ToListAsync();

            return View(citas);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelarCita(int folio)
        {
            try
            {
                var cita = await _context.Citas
                    .FirstOrDefaultAsync(c => c.FolioCitas == folio);

                if (cita == null)
                {
                    return Json(new { ok = false, mensaje = "La cita no existe." });
                }

                var estatus = cita.estatusAtencion?.Trim();

                // Ya atendida
                if (string.Equals(estatus, "Atendida", StringComparison.OrdinalIgnoreCase))
                {
                    return Json(new
                    {
                        ok = false,
                        mensaje = "No se puede cancelar una cita que ya fue atendida."
                    });
                }

                // Ya cancelada (cualquier tipo)
                if (estatus != null &&
                    estatus.Contains("Cancelada", StringComparison.OrdinalIgnoreCase))
                {
                    return Json(new
                    {
                        ok = false,
                        mensaje = "La cita ya se encuentra cancelada."
                    });
                }

              
                cita.estatusAtencion = "Cancelada por el Doctor";

                await _context.SaveChangesAsync();

                return Json(new
                {
                    ok = true,
                    mensaje = "La cita ha sido cancelada con éxito."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    ok = false,
                    mensaje = "ERROR SERVIDOR: " + ex.Message
                });
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

            // Paciente logueado o pacientes segun sea el rol

            // ID del usuario logueado
            var userId = int.Parse(User?.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var role = User?.FindFirst(ClaimTypes.Role)?.Value;

            if (!role.Equals("Paciente"))
            {
                ViewBag.Paciente = _context.Pacientes
                                            .Include(d => d.IdUsuarioNavigation)
                                            .Select(d => new SelectListItem
                                            {
                                                Value = d.IdPaciente.ToString(),
                                                Text = ("[" + d.IdUsuarioNavigation.NombreUsuario + "] " + d.Nombre + " " + d.ApellidoP + " " + d.ApellidoM)
                                            })
                                            .ToList();
            }
            else
            {
                var paciente = _context.Pacientes.FirstOrDefault(p => p.IdUsuario == (userId));

                ViewBag.PacienteNombre = paciente?.Nombre;
                ViewBag.PacienteId = paciente?.IdPaciente;
            }
                
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
