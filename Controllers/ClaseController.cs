using AplicacionWebEscolar.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace AplicacionWebEscolar.Controllers
{
    public class ClaseController : Controller
    {
        public String uri = System.Configuration.ConfigurationManager.ConnectionStrings["APIEscolar"].ToString();
        public async Task<ActionResult> Index()
        {
            List<Clase> listaClases = await ObtenerClases();
            
            foreach (Clase clase in listaClases)
            {
                using (var maestroC = new MaestroController())
                {
                    Maestro maestro = await maestroC.ObtenerMaestro(clase.Maestro.Id);
                    clase.Maestro.Nombre = maestro.Nombre + " " + maestro.Apellidos;
                }
                using (var materiaC = new MateriaController())
                {
                    Materia materia = await materiaC.ObtenerMateria(clase.Materia.Id);
                    clase.Materia.Nombre = materia.Nombre;
                }
                using (var horarioC = new HorarioController())
                {
                    Horario horario = await horarioC.ObtenerHorario(clase.Horario.Id);
                    clase.Horario.Nombre = horario.Nombre;
                }
            }
            return View(listaClases);
            
        }

        public async Task<ActionResult> Details(int id)
        {
            Clase clase = await ObtenerClase(id);
            
            return View(clase);
        }

        public async Task<ActionResult> Create()
        {
            ViewBag.ListadoItemsMaestros = await GenerarItemsMaestros();
            ViewBag.ListadoItemsMaterias = await GenerarItemsMaterias();
            ViewBag.ListadoItemsHorarios = await GenerarItemsHorarios();

            return View();

        }

        [HttpPost]
        public async Task<ActionResult> Create(Clase clase)
        {
            if (clase.Maestro != null)
            {
                using (var maestroC = new MaestroController()) { clase.Maestro = await maestroC.ObtenerMaestro(clase.Maestro.Id); }
                using (var materiaC = new MateriaController()) { clase.Materia = await materiaC.ObtenerMateria(clase.Materia.Id); }
                using (var horarioC = new HorarioController()) { clase.Horario = await horarioC.ObtenerHorario(clase.Horario.Id); }
            }
            if (ModelState.IsValid)
            {
                if (await GuardarClase(clase, true))
                    return RedirectToAction("Index");
            }
            return View("Index");
        }

        public async Task<ActionResult> Edit(int id)
        {
            Clase clase = await ObtenerClase(id);

            ViewBag.ListadoItemsMaestros = await GenerarItemsMaestros();
            ViewBag.ListadoItemsMaterias = await GenerarItemsMaterias();
            ViewBag.ListadoItemsHorarios = await GenerarItemsHorarios();

            return View(clase);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(Clase clase, FormCollection collection)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (await GuardarClase(clase, false))
                        return RedirectToAction("Index");
                }
                return View("Index");
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> Delete(int id)
        {
            Clase clase = await ObtenerClase(id);

            using (var maestroC = new MaestroController())
            {
                Maestro maestro = await maestroC.ObtenerMaestro(clase.Maestro.Id);
                clase.Maestro = maestro;
            }
            using (var materiaC = new MateriaController())
            {
                Materia materia = await materiaC.ObtenerMateria(clase.Materia.Id);
                clase.Materia = materia;
            }
            using (var horarioC = new HorarioController())
            {
                Horario horario = await horarioC.ObtenerHorario(clase.Horario.Id);
                clase.Horario = horario;
            }

            return View(clase);
        }

        [HttpPost]
        public async Task<ActionResult> Delete(Clase clase)
        {
            clase = await ObtenerClase(clase.Id);

            if (await EliminarClase(clase))
                return RedirectToAction("Index");
            
            return View();
        }

        private async Task<List<SelectListItem>> GenerarItemsMaestros()
        {
            using (var maestroC = new MaestroController())
            {
                List<SelectListItem> lMaestros = new List<SelectListItem>();
            
                List<Maestro> listaMaestros = await maestroC.ObtenerMaestros();

                lMaestros.Add(new SelectListItem { Text = "Seleccione", Value = "" });

                foreach (Maestro maestro in listaMaestros)
                {
                    lMaestros.Add(new SelectListItem { Text = maestro.Nombre + " " + maestro.Apellidos, Value = maestro.Id.ToString() });
                }

                return lMaestros;
            }
        }

        private async Task<List<SelectListItem>> GenerarItemsMaterias()
        {
            using (var materiaC = new MateriaController())
            {
                List<SelectListItem> lMaterias = new List<SelectListItem>();

                List<Materia> listaMaterias = await materiaC.ObtenerMaterias();

                lMaterias.Add(new SelectListItem { Text = "Seleccione", Value = "" });

                foreach (Materia materia in listaMaterias)
                {
                    lMaterias.Add(new SelectListItem { Text = materia.Nombre, Value = materia.Id.ToString() });
                }

                return lMaterias;
            }
        }

        private async Task<List<SelectListItem>> GenerarItemsHorarios()
        {
            using (var horarioC = new HorarioController())
            {
                List<SelectListItem> lHorarios = new List<SelectListItem>();

                List<Horario> listaHorarios = await horarioC.ObtenerHorarios();

                lHorarios.Add(new SelectListItem { Text = "Seleccione", Value = "" });

                foreach (Horario horario in listaHorarios)
                {
                    lHorarios.Add(new SelectListItem { Text = horario.Nombre, Value = horario.Id.ToString() });
                }

                return lHorarios;
            }
        }

        private async Task<List<Clase>> ObtenerClases()
        {
            using (var httpClient = new HttpClient())
            {
                var json = await httpClient.GetStringAsync(string.Format("{0}/clase", uri));
                List<Clase> clases = JsonConvert.DeserializeObject<List<Clase>>(json).Where(c => c.Estatus == true).Select(c => c).ToList();

                return clases;
            }
        }

        private async Task<Clase> ObtenerClase(int id)
        {
            using (var httpClient = new HttpClient())
            {
                var json = await httpClient.GetStringAsync(string.Format("{0}/clase/{1}", uri,id.ToString()));
                Clase clase = JsonConvert.DeserializeObject<Clase>(json);

                return clase;
            }
        }
        
        private async Task<bool> GuardarClase(Clase clase, bool nuevo)
        {
            using (var client = new HttpClient())
            {
                var strNJsonInsert = new
                {
                    nombre = clase.Nombre,
                    maestro = new
                    {
                        id = clase.Maestro.Id,
                        nombre = clase.Maestro.Nombre,
                        apellidos = clase.Maestro.Apellidos,
                        telefono = clase.Maestro.Telefono,
                        email = clase.Maestro.Email,
                        estatus = clase.Maestro.Estatus
                    },
                    materia = new
                    {
                        id = clase.Materia.Id,
                        nombre = clase.Materia.Nombre,
                        estatus = clase.Materia.Estatus
                    },
                    horario = new
                    {
                        id = clase.Horario.Id,
                        nombre = clase.Horario.Nombre,
                        estatus = clase.Horario.Estatus
                    },
                    maxAlumnos = clase.MaxAlumnos,
                    estatus = clase.Estatus
                };

                var strNJsonUpdate = new
                {
                    id = clase.Id,
                    nombre = clase.Nombre,
                    maestro = new
                    {
                        id = clase.Maestro.Id,
                        nombre = clase.Maestro.Nombre,
                        apellidos = clase.Maestro.Apellidos,
                        telefono = clase.Maestro.Telefono,
                        email = clase.Maestro.Email,
                        estatus = clase.Maestro.Estatus
                    },
                    materia = new
                    {
                        id = clase.Materia.Id,
                        nombre = clase.Materia.Nombre,
                        estatus = clase.Materia.Estatus
                    },
                    horario = new
                    {
                        id = clase.Horario.Id,
                        nombre = clase.Horario.Nombre,
                        estatus = clase.Horario.Estatus
                    },
                    maxAlumnos = clase.MaxAlumnos,
                    estatus = clase.Estatus
                };
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                string json = (nuevo) ? javaScriptSerializer.Serialize(strNJsonInsert) : javaScriptSerializer.Serialize(strNJsonUpdate);

                client.BaseAddress = new Uri(uri);

                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(string.Format("{0}/clase", uri), stringContent);

                return response.IsSuccessStatusCode;
            }
        }

        private async Task<bool> EliminarClase(Clase clase)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(uri);

                var response = await client.DeleteAsync(string.Format("{0}/clase/{1}", uri, clase.Id.ToString()));

                return response.IsSuccessStatusCode;
            }
        }
    }
}
