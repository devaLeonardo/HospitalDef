using System;
using System.Collections.Generic;

namespace HospitalDef.Models;

public partial class DetallesMedicamento
{
    public int IdDm { get; set; }

    public int? FolioTicket { get; set; }

    public int? IdMedicamento { get; set; }

    public decimal Precio { get; set; }

    public int Cantidad { get; set; }

    public decimal SubTotal { get; set; }

    public virtual Ticket? FolioTicketNavigation { get; set; }

    public virtual Medicamento? IdMedicamentoNavigation { get; set; }
}
