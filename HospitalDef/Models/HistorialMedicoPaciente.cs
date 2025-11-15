using System;
using System.Collections.Generic;

namespace HospitalDef.Models;

public partial class HistorialMedicoPaciente
{
    public int IdHistorialMedicoPaciente { get; set; }

    public int? IdPaciente { get; set; }

    public decimal Peso { get; set; }

    public decimal Altura { get; set; }

    public string Alergias { get; set; } = null!;

    public string TipoSangre { get; set; } = null!;

    public string Padecimientos { get; set; } = null!;

    public virtual Paciente? IdPacienteNavigation { get; set; }
}
