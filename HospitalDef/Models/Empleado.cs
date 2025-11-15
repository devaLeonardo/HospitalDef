using System;
using System.Collections.Generic;

namespace HospitalDef.Models;

public partial class Empleado
{
    public int IdEmpleado { get; set; }

    public int? IdUsuario { get; set; }

    public int? IdHorario { get; set; }

    public string Rfc { get; set; } = null!;

    public string CuentaBancaria { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string ApellidoP { get; set; } = null!;

    public string ApellidoM { get; set; } = null!;

    public DateOnly FechaNacimiento { get; set; }

    public string Sexo { get; set; } = null!;

    public int Edad { get; set; }

    public string Calle { get; set; } = null!;

    public string Colonia { get; set; } = null!;

    public string Municipio { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public bool? Activo { get; set; }

    public virtual ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();

    public virtual ICollection<Farmaceutico> Farmaceuticos { get; set; } = new List<Farmaceutico>();

    public virtual HorarioEmpleado? IdHorarioNavigation { get; set; }

    public virtual Usuario? IdUsuarioNavigation { get; set; }

    public virtual ICollection<Recepcionistum> Recepcionista { get; set; } = new List<Recepcionistum>();
}
