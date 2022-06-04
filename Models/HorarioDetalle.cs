using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AplicacionWebEscolar.Models
{
    public class HorarioDetalle
    {
        public int Id { get; set; }
        public Horario Horario { get; set; }
        
        [Required]
        [Range(1, 7, ErrorMessage = "Debe capturar un número entre 1 y 7")]
        [RegularExpression("([1-7]+)", ErrorMessage = "Favor de capturar un número válido")]
        public int diaSemana { get; set; }
        public String horaInicio { get; set; }
        public String horaFin { get; set; }

    }
}