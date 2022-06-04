using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AplicacionWebEscolar.Models
{
    public class Materia 
    { 
        public int Id { get; set; }
        public string Nombre { get; set; }
        public bool Estatus { get; set; }
    }
}