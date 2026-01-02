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
    public class TicketsController : Controller
    {
        private readonly HospitalContext _context;

        public TicketsController(HospitalContext context)
        {
            _context = context;
        }

        // GET: Tickets
        public async Task<IActionResult> Index()
        {
            var hospitalContext = _context.Tickets.Include(t => t.IdUsuarioNavigation);
            return View(await hospitalContext.ToListAsync());
        }

        [HttpGet]
        public IActionResult BuscarMedicamentos(string termino)
        {
            if (string.IsNullOrEmpty(termino) || termino.Length < 2)
            {
                return Json(new List<object>());
            }

            var listaResultados = _context.SP_getMedicamentos_Servicios(termino).ToList();

            return Json(listaResultados);
        }





        public class TicketDTO
        {
            public decimal Total { get; set; }
            public List<DetalleItemDTO> Items { get; set; }
        }

        public class DetalleItemDTO
        {
            public string Id { get; set; }
            public string Nombre { get; set; }
            public decimal Precio { get; set; }
            public string Tipo { get; set; } 
        }


        [HttpPost]
        public async Task<IActionResult> GuardarTicket([FromBody] TicketDTO datos)
        {
            if (datos == null || datos.Items == null || !datos.Items.Any())
                return Json(new { success = false, mensaje = "Carrito vacío" });




            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (!User.Identity.IsAuthenticated)
                    return NotFound();

                // ID del usuario logueado
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                int tiempoEntero = int.Parse(DateTime.Now.ToString("HHmm"));

                var nuevoTicket = new Ticket
                {
                    Fecha = DateOnly.FromDateTime(DateTime.Now),
                    Hora = TimeOnly.FromDateTime(DateTime.Now),
                    IdUsuario = userId,
                    NumCliente = userId+tiempoEntero,
                    Total = datos.Total
                };
                _context.Tickets.Add(nuevoTicket);
                await _context.SaveChangesAsync();

                var itemsAgrupados = datos.Items
                    .GroupBy(i => new { i.Id, i.Tipo })
                    .Select(g => new {
                        Id = g.Key.Id,
                        Tipo = g.Key.Tipo,
                        Nombre = g.First().Nombre,
                        Precio = g.First().Precio,
                        Cantidad = g.Count() 
                    });

                foreach (var grupo in itemsAgrupados)
                {
                    if (grupo.Tipo == "Medicamento")
                    {
                        _context.DetallesMedicamentos.Add(new DetallesMedicamento
                        {
                            FolioTicket = nuevoTicket.FolioTicket,
                            IdMedicamento = int.Parse(grupo.Id),
                            Precio = grupo.Precio,
                            Cantidad = grupo.Cantidad, 
                            SubTotal = grupo.Precio * grupo.Cantidad
                        });
                    }
                    else 
                    {
                        _context.DetallesServicios.Add(new DetallesServicio
                        {
                            FolioTicket = nuevoTicket.FolioTicket,
                            IdServicio = int.Parse(grupo.Id),
                            Precio = grupo.Precio,
                            SubTotal = grupo.Precio * grupo.Cantidad
                        });
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Json(new { success = true, folio = nuevoTicket.FolioTicket });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Json(new { success = false, mensaje = ex.Message });
            }
        }




        public async Task<IActionResult> generarPDF(int id)
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjGyl/Vkd+XU9FcVRDQmJKYVF2R2VJelRyfF9FZ0wxOX1dQl9mSHxSckRkW3ZeeHRWQWFXUkU=");

            var ticket = await _context.Tickets
                .Include(t => t.DetallesMedicamentos)
                .Include(t => t.DetallesServicios)
                .FirstOrDefaultAsync(t => t.FolioTicket == id);


            PdfDocument pdfDocument = new PdfDocument();
            PdfPage currentPage = pdfDocument.Pages.Add();
            SizeF clientSize = currentPage.GetClientSize();



            PdfFont font = new PdfStandardFont(PdfFontFamily.Courier, 10);
            var text = new PdfTextElement("TICKET DE COMPRA", font);
            text.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);
            PdfLayoutResult result = text.Draw(currentPage, new PointF(250, 13 + 10));


            font = new PdfStandardFont(PdfFontFamily.Courier, 10);
            text = new PdfTextElement("RFC de la empresa : #123456789", font);
            text.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);
            result = text.Draw(currentPage, new PointF(250, result.Bounds.Bottom + 10));



            font = new PdfStandardFont(PdfFontFamily.Courier, 10);
            text = new PdfTextElement("Folio ticket: #"+ticket.FolioTicket, font);
            result = text.Draw(currentPage, new PointF(14, result.Bounds.Bottom + 10));

            font = new PdfStandardFont(PdfFontFamily.Courier, 10);
            text = new PdfTextElement("FECHA DE COMPRA: "+ ticket.Fecha.ToString("dd/MM/yyyy"), font);
            result = text.Draw(currentPage, new PointF(14, result.Bounds.Bottom + 10));


            //linea divisora
            PdfGraphics graphics2 = currentPage.Graphics;
            PdfPen bluePen = new PdfPen(Color.FromArgb(0, 0, 0), 2f);
            graphics2.DrawLine(bluePen, new PointF(14, result.Bounds.Bottom + 15), new PointF(clientSize.Width - 14, result.Bounds.Bottom + 15));

            //contenido de la receta

            var medicamentoservicios = _context.SP_obtenerDetalles(ticket.FolioTicket).ToList();
            



            font = new PdfStandardFont(PdfFontFamily.Courier, 10);
            text = new PdfTextElement("Productos y servicios", font);
            result = text.Draw(currentPage, new PointF(14, result.Bounds.Bottom + 20));


            foreach (var medicamentoservicio in medicamentoservicios)
            {
                font = new PdfStandardFont(PdfFontFamily.Courier, 10);
                text = new PdfTextElement(medicamentoservicio.nombre + ", cantidad: "+ medicamentoservicio.cantidad + " .......................... " + medicamentoservicio.subtotal, font);
                result = text.Draw(currentPage, new PointF(24, result.Bounds.Bottom + 10));
            }

            //linea divisora
            graphics2.DrawLine(bluePen, new PointF(14, result.Bounds.Bottom + 15), new PointF(clientSize.Width - 14, result.Bounds.Bottom + 15));

            font = new PdfStandardFont(PdfFontFamily.Courier, 10);
            text = new PdfTextElement("TOTAL A PAGAR: " + ticket.Total, font);
            result = text.Draw(currentPage, new PointF(14, result.Bounds.Bottom + 20));






            // Aqui se guarda el documento PDF en un MemoryStream
            MemoryStream stream = new MemoryStream();
            pdfDocument.Save(stream);
            pdfDocument.Close(true);
            stream.Position = 0;

            string nombreArchivo = "ticket.pdf";

            return File(stream, "application/pdf", nombreArchivo);
        }








        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.IdUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.FolioTicket == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/Create
        public IActionResult Create()
        {
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FolioTicket,IdUsuario,Fecha,Hora,NumCliente,Total")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario", ticket.IdUsuario);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario", ticket.IdUsuario);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FolioTicket,IdUsuario,Fecha,Hora,NumCliente,Total")] Ticket ticket)
        {
            if (id != ticket.FolioTicket)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.FolioTicket))
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
            ViewData["IdUsuario"] = new SelectList(_context.Usuarios, "IdUsuario", "IdUsuario", ticket.IdUsuario);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.IdUsuarioNavigation)
                .FirstOrDefaultAsync(m => m.FolioTicket == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.FolioTicket == id);
        }
    }
}
