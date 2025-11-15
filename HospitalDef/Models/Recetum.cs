using System;
using System.Collections.Generic;

namespace HospitalDef.Models;

public partial class Recetum
{
    public int FolioReceta { get; set; }

    public int? FolioCita { get; set; }

    public string Observaciones { get; set; } = null!;

    public string Tratamientos { get; set; } = null!;

    public string Diagnostico { get; set; } = null!;

    public virtual Cita? FolioCitaNavigation { get; set; }
}
