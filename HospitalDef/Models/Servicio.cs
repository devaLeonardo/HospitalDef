using System;
using System.Collections.Generic;

namespace HospitalDef.Models;

public partial class Servicio
{
    public int Idservicio { get; set; }

    public string NombreServicio { get; set; } = null!;

    public decimal Precio { get; set; }

    public TimeSpan HoraI { get; set; }

    public TimeSpan HoraF { get; set; }

    public string Estatus { get; set; } = null!;

    public virtual ICollection<DetallesServicio> DetallesServicios { get; set; } = new List<DetallesServicio>();
}
