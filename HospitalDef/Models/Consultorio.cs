using System;
using System.Collections.Generic;

namespace HospitalDef.Models;

public partial class Consultorio
{
    public int IdConsultorio { get; set; }

    public int Planta { get; set; }

    public string Numero { get; set; } = null!;

    public string Edificio { get; set; } = null!;

    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
}
