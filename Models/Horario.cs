using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace AplicacionWebEscolar.Models
{
    public class Horario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public bool Estatus { get; set; }
        public ICollection<HorarioDetalle> Detalles { get; set; }
        
        public Horario()
        {
            this.Detalles = new HashSet<HorarioDetalle>();
        }
    }
}