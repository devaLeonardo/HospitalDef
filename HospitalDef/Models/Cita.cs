using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HospitalDef.Models;

public partial class Cita
{
    public int FolioCitas { get; set; }

    public int? idPaciente { get; set; }

    public int? idDoctor { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTime fechaCreacionCita { get; set; }

    public DateTime fechaCita { get; set; }

    public TimeOnly horaCita { get; set; }

    public TimeOnly horaInicio { get; set; }

    public TimeOnly horaTermino { get; set; }

    public string estatusAtencion { get; set; } = null!;


    public virtual ICollection<BitacoraCita> BitacoraCita { get; set; } = new List<BitacoraCita>();

    public virtual Doctor? IdDoctorNavigation { get; set; }

    public virtual Paciente? IdPacienteNavigation { get; set; }

    public virtual ICollection<Recetum> Receta { get; set; } = new List<Recetum>();

    [NotMapped]//permite mantener la vista pero no lo mapea en la base de datos
    [Display(Name = "Especialidad")]
    [Required(ErrorMessage = "Debe seleccionar una especialidad.")]
    public int EspecialidadIdFiltro { get; set; }



}
