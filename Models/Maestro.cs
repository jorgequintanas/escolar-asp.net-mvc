using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AplicacionWebEscolar.Models
{
    public class Maestro
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public bool Estatus { get; set; }
    }
}