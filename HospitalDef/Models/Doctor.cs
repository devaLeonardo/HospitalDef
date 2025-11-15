using System;
using System.Collections.Generic;

namespace HospitalDef.Models;

public partial class Doctor
{
    public int IdDoctor { get; set; }

    public int? IdEmpleado { get; set; }

    public int? IdEspecialidad { get; set; }

    public int? IdConsultorio { get; set; }

    public string? CedulaProf { get; set; }

    public virtual ICollection<Cita> Cita { get; set; } = new List<Cita>();

    public virtual Consultorio? IdConsultorioNavigation { get; set; }

    public virtual Empleado? IdEmpleadoNavigation { get; set; }

    public virtual Especialidade? IdEspecialidadNavigation { get; set; }
}
