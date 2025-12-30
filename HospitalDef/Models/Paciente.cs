using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalDef.Models;

public partial class Paciente
{
    public int IdPaciente { get; set; }

    public int? IdUsuario { get; set; }

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

    public virtual ICollection<Cita> Cita { get; set; } = new List<Cita>();

    public virtual ICollection<HistorialMedicoPaciente> HistorialMedicoPacientes { get; set; } = new List<HistorialMedicoPaciente>();

    public virtual Usuario? IdUsuarioNavigation { get; set; }


    [NotMapped]
    public string NombreCompleto => $"{"["} {IdPaciente} {"]"} {Nombre} {ApellidoP} {ApellidoM}";
}
