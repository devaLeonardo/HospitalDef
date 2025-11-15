using System;
using System.Collections.Generic;

namespace HospitalDef.Models;

public partial class HistorialCitasMedicoPaciente
{
    public int IdHcmp { get; set; }

    public DateTime Fecha { get; set; }

    public TimeOnly Hora { get; set; }
}
