using System.ComponentModel.DataAnnotations;

namespace HospitalDef.Models
{
    public class spMED_SER
    {
        public int id { get; set; }
        public int idTicket { get; set; }
        public string nombre { get; set; }
        public decimal precio { get; set; }
        public int cantidad { get; set; }
        public decimal subtotal { get; set; }
        public string tipo { get; set; }
    }
}