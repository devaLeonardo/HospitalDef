namespace HospitalDef.Models
{
    public class VistaDoctor
    {
        public int IdDoctor { get; set; }
        public string NombreCompleto { get; set; }
        public string Nombre { get; set; }
        public string ApellidoP { get; set; }
        public string ApellidoM { get; set; }
        public string Usuario { get; set; }
        public string Contraseña { get; set; }
        public string CuentaBancaria { get; set; }
        public string Especialidad { get; set; }
        public string CedulaProf { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public string Consultorio { get; set; }
        public int? IdConsultorio { get; set; }
        public bool? Activo { get; set; }

    }
}
