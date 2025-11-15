using System;
using System.Collections.Generic;

namespace HospitalDef.Models;

public partial class Ticket
{
    public int FolioTicket { get; set; }

    public int? IdUsuario { get; set; }

    public DateOnly Fecha { get; set; }

    public TimeOnly Hora { get; set; }

    public int NumCliente { get; set; }

    public decimal Total { get; set; }

    public virtual ICollection<DetallesMedicamento> DetallesMedicamentos { get; set; } = new List<DetallesMedicamento>();

    public virtual ICollection<DetallesServicio> DetallesServicios { get; set; } = new List<DetallesServicio>();

    public virtual Usuario? IdUsuarioNavigation { get; set; }
}
