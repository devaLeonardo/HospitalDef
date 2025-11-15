using System;
using System.Collections.Generic;

namespace HospitalDef.Models;

public partial class BitacoraCita
{
    public int IdBitacoraCitas { get; set; }

    public int? FolioCitas { get; set; }

    public string Estatus { get; set; } = null!;

    public DateTime Fecha { get; set; }

    public decimal Costo { get; set; }

    public string PoliticaCancelacion { get; set; } = null!;

    public decimal MontoDevuelto { get; set; }

    public virtual Cita? FolioCitasNavigation { get; set; }
}
