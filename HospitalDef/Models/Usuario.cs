using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HospitalDef.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string Correo { get; set; } = null!;

    public string Contraseña { get; set; } = null!;

    public string NombreUsuario { get; set; } = null!;
    [Required]
    [RegularExpression(@"^\d+$", ErrorMessage = "El teléfono solo debe contener números.")]

    public string Telefono { get; set; } = null!;

    public DateTime? FechaRegistro { get; set; }

    public bool? Activo { get; set; }

    public virtual ICollection<Empleado> Empleados { get; set; } = new List<Empleado>();

    public virtual ICollection<Paciente> Pacientes { get; set; } = new List<Paciente>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
