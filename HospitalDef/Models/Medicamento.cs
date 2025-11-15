using System;
using System.Collections.Generic;

namespace HospitalDef.Models;

public partial class Medicamento
{
    public int IdMedicamento { get; set; }

    public string NombreMedicamento { get; set; } = null!;

    public string Lote { get; set; } = null!;

    public DateOnly Caducidad { get; set; }

    public decimal PrecioCompra { get; set; }

    public decimal PrecioVenta { get; set; }

    public string Ubicacion { get; set; } = null!;

    public int Stock { get; set; }

    public virtual ICollection<DetallesMedicamento> DetallesMedicamentos { get; set; } = new List<DetallesMedicamento>();
}
