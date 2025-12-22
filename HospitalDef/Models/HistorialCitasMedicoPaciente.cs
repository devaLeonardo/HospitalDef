using System;
using System.Collections.Generic;

namespace HospitalDef.Models;

public partial class HistorialCitasMedicoPaciente
{
    public int IdHcmp { get; set; }

    public DateTime Fecha { get; set; }

    public TimeOnly Hora { get; set; }
    public string? Estatus { get; set; }
    public int? IdCita { get; set; }
    public string? Doctor { get; set; }
    public string? Consultorio { get; set; }
    public string? Paciente { get; set; }
}
