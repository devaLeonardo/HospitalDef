using System;
using System.Collections.Generic;

namespace HospitalDef.Models;

public partial class Especialidade
{
    public int IdEspecialidad { get; set; }

    public string Especialidades { get; set; } = null!;

    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}
