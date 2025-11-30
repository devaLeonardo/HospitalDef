namespace HospitalDef.Models
{
    public class VistaDoctorHorario
    {
        public int IdDoctor { get; set; }
        public string NombreCompleto { get; set; }
        public string Especialidad { get; set; }
        public string DiaSemana { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFin { get; set; } 
        public string Consultorio { get; set; }
    }
}
