using System;

namespace HospitalDef.Models.ViewModels
{
    public class HistorialMedicoPacienteVM
    {
        public int IdBitacoraCitas { get; set; }
        public DateTime FechaMovimiento { get; set; }

        // Doctor
        public int IdDoctor { get; set; }
        public string UsuarioDoctor { get; set; }
        public string NombreDoctor { get; set; }

        // Especialidad
        public string Especialidad { get; set; }

        // Paciente
        public int IdPaciente { get; set; }
        public string UsuarioPaciente { get; set; }
        public string NombrePaciente { get; set; }

        // Receta
        public string Observaciones { get; set; }
        public string Tratamientos { get; set; }
        public string Diagnostico { get; set; }

        // Consultorio
        public string Consultorio { get; set; }
    }
}

