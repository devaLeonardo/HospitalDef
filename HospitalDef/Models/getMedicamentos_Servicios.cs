using System.ComponentModel.DataAnnotations;

namespace HospitalDef.Models
{
    public class getMedicamentos_Servicios
    {
        public int id { get; set; }
        public string nombre { get; set; }
        public decimal precio { get; set; }
        public string tipo { get; set; }
    }
}
