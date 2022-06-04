using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AplicacionWebEscolar.Models
{
    public class Clase
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }
        
        public Maestro Maestro { get; set; }
        public Materia Materia { get; set; }

        public Horario Horario { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "Debe capturar un número entre 1 y 100")]
        [RegularExpression("([0-9]+)", ErrorMessage = "Favor de capturar un número válido")]
        public int MaxAlumnos { get; set; }
        
        public bool Estatus { get; set; }
    }
}