using System;
using System.Collections.Generic;

namespace HospitalDef.Models;

public partial class DetallesServicio
{
    public int IdDs { get; set; }

    public int? FolioTicket { get; set; }

    public int? IdServicio { get; set; }

    public decimal Precio { get; set; }

    public decimal SubTotal { get; set; }

    public virtual Ticket? FolioTicketNavigation { get; set; }

    public virtual Servicio? IdServicioNavigation { get; set; }
}
