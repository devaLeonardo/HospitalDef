using System;
using System.Collections.Generic;

namespace HospitalDef.Models;

public partial class HorarioEmpleado
{
    public int IdHorario { get; set; }

    public TimeOnly HoraEntrada { get; set; }

    public TimeOnly HoraSalida { get; set; }

    public string Dias { get; set; } = null!;

    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();
}
