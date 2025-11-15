using System;
using System.Collections.Generic;

namespace HospitalDef.Models;

public partial class Farmaceutico
{
    public int IdFarmaceutico { get; set; }

    public int? IdEmpleado { get; set; }

    public virtual Empleado? IdEmpleadoNavigation { get; set; }
}
