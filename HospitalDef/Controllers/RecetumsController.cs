using HospitalDef.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HospitalDef.Controllers
{
    public class RecetumsController : Controller
    {
        private readonly HospitalContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;


        public RecetumsController(HospitalContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment; // <-- ¡Inyección!

        }

        // GET: Recetums
        public async Task<IActionResult> Index()
        {
            var hospitalContext = _context.Receta.Include(r => r.FolioCitaNavigation);
            return View(await hospitalContext.ToListAsync());
        }

        // GET: Recetums/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recetum = await _context.Receta
                .Include(r => r.FolioCitaNavigation)
                .FirstOrDefaultAsync(m => m.FolioReceta == id);
            if (recetum == null)
            {
                return NotFound();
            }

            return View(recetum);
        }

        // GET: Recetums/Create
        public IActionResult Create(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            




                var cita =_context.Citas
                .FirstOrDefault(c => c.FolioCitas == id);

            ViewBag.FolioCita = cita?.FolioCitas;





            return View();
        }

        // POST: Recetums/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FolioReceta,FolioCita,Observaciones,Tratamientos,Diagnostico")] Recetum recetum)
        {


            
            if (ModelState.IsValid)
            {
               _context.Add(recetum);
               await _context.SaveChangesAsync();

            return RedirectToAction("Index","Citas");
        }

           

            return View(recetum);
        }
        




        public async Task<IActionResult> generarPDF(int id)
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjGyl/Vkd+XU9FcVRDQmJKYVF2R2VJelRyfF9FZ0wxOX1dQl9mSHxSckRkW3ZeeHRWQWFXUkU=");


            


            var recetum = await _context.Receta
                .Include(r => r.FolioCitaNavigation)
                .ThenInclude(c => c.IdPacienteNavigation)
                .FirstOrDefaultAsync(m => m.FolioCitaNavigation.FolioCitas == id);

            var fecha = recetum?.FolioCitaNavigation?.fechaCita.ToString("dd/MM/yyyy");

            DateTime? fechaCita = recetum?.FolioCitaNavigation?.fechaCita;
            DateTime hoy = DateTime.Today;

            if (fechaCita.HasValue)
            {
                if (fechaCita.Value.Date != hoy)
                {
                    return BadRequest("No se puede generar la receta. La fecha de la cita no es hoy.");
                }
            }




                PdfDocument pdfDocument = new PdfDocument();
            PdfPage currentPage = pdfDocument.Pages.Add();
            SizeF clientSize = currentPage.GetClientSize();

            //aqui se agrega la imagen

            string webRootPath = _hostEnvironment.WebRootPath; // Obtiene la ruta a wwwroot
            string rutaFisicaImagen = Path.Combine(webRootPath, "images", "logo.PNG");

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
            text = new PdfTextElement("Receta", font);
            text.StringFormat = new PdfStringFormat(PdfTextAlignment.Right);
            result = text.Draw(currentPage, new PointF(clientSize.Width - 250, iconLocation.Y + 20));



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



            //parte central de la receta tiene datos del doctor

            var doctor = recetum.FolioCitaNavigation.idDoctor;
            var doctorName = await _context.Doctors
                .Include(c=>c.IdEmpleadoNavigation)
                .Include(c => c.IdEspecialidadNavigation)
                .FirstOrDefaultAsync(c=>c.IdDoctor==doctor);
            

            font = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
            text = new PdfTextElement("Dr. " + doctorName.IdEmpleadoNavigation.Nombre+" "+ doctorName.IdEmpleadoNavigation.ApellidoP, font);
            text.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);
            result = text.Draw(currentPage, new PointF(250, result.Bounds.Bottom + 10));


            font = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
            text = new PdfTextElement("Cedula Profesional: "+doctorName.CedulaProf, font);
            text.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);
            result = text.Draw(currentPage, new PointF(250, result.Bounds.Bottom + 10));

            font = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
            text = new PdfTextElement("Especialidad: "+doctorName.IdEspecialidadNavigation.Especialidades, font);
            text.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);
            result = text.Draw(currentPage, new PointF(250, result.Bounds.Bottom + 10));


            //datos del paciente

            font = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
            text = new PdfTextElement("Paciente: "+recetum.FolioCitaNavigation.IdPacienteNavigation.Nombre+" "
                + recetum.FolioCitaNavigation.IdPacienteNavigation.ApellidoP, font);
            result = text.Draw(currentPage, new PointF(14, result.Bounds.Bottom + 20));

            font = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
            text = new PdfTextElement("Folio Cita: "+recetum.FolioCitaNavigation.FolioCitas, font);
            result = text.Draw(currentPage, new PointF(14, result.Bounds.Bottom + 10));

            font = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
            text = new PdfTextElement("Folio Receta: "+recetum.FolioReceta, font);
            result = text.Draw(currentPage, new PointF(14, result.Bounds.Bottom + 10));


            font = new PdfStandardFont(PdfFontFamily.Helvetica, 8);
            text = new PdfTextElement("Este documento no es valido sin firma y sello del doctor", font);
            result = text.Draw(currentPage, new PointF(14+300, result.Bounds.Bottom));


            //linea divisora
            PdfGraphics graphics2 = currentPage.Graphics;
            PdfPen bluePen = new PdfPen(Color.FromArgb(53, 67, 168), 2f);
            graphics2.DrawLine(bluePen, new PointF(14, result.Bounds.Bottom + 15), new PointF(clientSize.Width - 14, result.Bounds.Bottom + 15));






            //contenido de la receta
            font = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
            text = new PdfTextElement("Medicamentos", font);
            result = text.Draw(currentPage, new PointF(14, result.Bounds.Bottom + 20));

            foreach (var line in recetum.Tratamientos.Split('\n'))
            {
                font = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
                text = new PdfTextElement("- "+line, font);
                result = text.Draw(currentPage, new PointF(14, result.Bounds.Bottom + 10));
            }

           

            //linea divisora
            graphics2.DrawLine(bluePen, new PointF(14, result.Bounds.Bottom + 15), new PointF(clientSize.Width - 14, result.Bounds.Bottom + 15));

            font = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
            text = new PdfTextElement("Tratamiento", font);
            result = text.Draw(currentPage, new PointF(14, result.Bounds.Bottom + 20));

            foreach (var line in recetum.Diagnostico.Split('\n'))
            {
                font = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
                text = new PdfTextElement("- " + line, font);
                result = text.Draw(currentPage, new PointF(14, result.Bounds.Bottom + 10));
            }



            //linea divisora para observaciones
            graphics2.DrawLine(bluePen, new PointF(14, result.Bounds.Bottom + 15), new PointF(clientSize.Width - 14, result.Bounds.Bottom + 15));

           

            font = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
            text = new PdfTextElement("Observaciones", font);
            result = text.Draw(currentPage, new PointF(14, result.Bounds.Bottom + 20));


            foreach (var line in recetum.Observaciones.Split('\n'))
            {
                font = new PdfStandardFont(PdfFontFamily.Helvetica, 10);
                text = new PdfTextElement(line, font);
                result = text.Draw(currentPage, new PointF(24, result.Bounds.Bottom + 10));

            }

            //FOOTER

            RectangleF bounds = new RectangleF(0, 0, pdfDocument.Pages[0].GetClientSize().Width, 50);

            PdfPageTemplateElement footer = new PdfPageTemplateElement(bounds);
            font = new PdfStandardFont(PdfFontFamily.Helvetica, 7);
            PdfBrush brush = new PdfSolidBrush(Color.Black);


            PdfPageNumberField pageNumber = new PdfPageNumberField(font, brush);

            PdfPageCountField count = new PdfPageCountField(font, brush);
            //Add the fields in composite fields.
            PdfCompositeField compositeField = new PdfCompositeField(font, brush, "Page {0} of {1}", pageNumber, count);
            compositeField.Bounds = footer.Bounds;
            compositeField.Draw(footer.Graphics, new PointF(470, 40));

            PdfPen signaturePen = new PdfPen(Color.Black, 0.5f);

            // Definimos la posición Y de la línea, que debe estar ligeramente por encima del texto de paginación.
            // Tu texto de paginación está en Y=40, así que pondremos la línea en Y=25.
            float lineaY = 25;
            float margenIzquierdo = 50; // Dejar un margen.
            float anchoLinea = 150;

            // DrawLine(pluma, X inicial, Y inicial, X final, Y final)
            footer.Graphics.DrawLine(
                signaturePen,
                margenIzquierdo,
                lineaY,
                margenIzquierdo + anchoLinea,
                lineaY
            );


            // 2. AGREGAR EL TEXTO DE FIRMA (JUSTO DEBAJO DE LA LÍNEA)
            // Usamos la misma fuente pequeña que definiste para la paginación (Helvetica, 7).
            // Reutilizamos el PdfBrush negro.

            PdfTextElement firmaText = new PdfTextElement("Sello y Firma \n\n (Sin esto el Documento no es valido)", font, brush);

            // Posicionamos el texto justo debajo de la línea (Y = lineaY + 2)
            firmaText.Draw(
                footer.Graphics,
                new PointF(margenIzquierdo + (anchoLinea / 2) - (20), lineaY + 2)
            // ^ Intentamos centrar el texto bajo la línea
            );



            pdfDocument.Template.Bottom = footer;




            // Aqui se guarda el documento PDF en un MemoryStream
            MemoryStream stream = new MemoryStream();
            pdfDocument.Save(stream);
            pdfDocument.Close(true);
            stream.Position = 0;

            string nombreArchivo = "Receta.pdf";

            return File(stream,"application/pdf",nombreArchivo);
        }






        // GET: Recetums/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recetum = await _context.Receta.FindAsync(id);
            if (recetum == null)
            {
                return NotFound();
            }
            ViewData["FolioCita"] = new SelectList(_context.Citas, "FolioCitas", "FolioCitas", recetum.FolioCita);
            return View(recetum);
        }

        // POST: Recetums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FolioReceta,FolioCita,Observaciones,Tratamientos,Diagnostico")] Recetum recetum)
        {
            if (id != recetum.FolioReceta)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recetum);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecetumExists(recetum.FolioReceta))
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
            ViewData["FolioCita"] = new SelectList(_context.Citas, "FolioCitas", "FolioCitas", recetum.FolioCita);
            return View(recetum);
        }

        // GET: Recetums/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recetum = await _context.Receta
                .Include(r => r.FolioCitaNavigation)
                .FirstOrDefaultAsync(m => m.FolioReceta == id);
            if (recetum == null)
            {
                return NotFound();
            }

            return View(recetum);
        }

        // POST: Recetums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recetum = await _context.Receta.FindAsync(id);
            if (recetum != null)
            {
                _context.Receta.Remove(recetum);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecetumExists(int id)
        {
            return _context.Receta.Any(e => e.FolioReceta == id);
        }
    }
}
