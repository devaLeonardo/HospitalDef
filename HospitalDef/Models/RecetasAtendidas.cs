using Microsoft.EntityFrameworkCore;

namespace HospitalDef.Models
{
    [Keyless]
    public class RecetasAtendidas
    {
        public int IdDoctor { get; set; }
        public string NombreCompletoDoctor { get; set; }
        public int NumReceta { get; set; }
        public string Diagnostico { get; set; }
        public string Tratamiento { get; set; }
        public string NombreCompletoPaciente { get; set; }
        public DateTime Fecha { get; set; }
    }
}
