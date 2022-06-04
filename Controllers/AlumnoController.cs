using AplicacionWebEscolar.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace AplicacionWebEscolar.Controllers
{
    public class AlumnoController : Controller
    {
        public String uri = System.Configuration.ConfigurationManager.ConnectionStrings["APIEscolar"].ToString();

        public async Task<ActionResult> Index()
        {
            List<Alumno> listaAlumnos = await ObtenerAlumnos();
            return View(listaAlumnos);
        }

        public async Task<ActionResult> Details(int id)
        {
            Alumno alumno = await ObtenerAlumno(id);
            return View(alumno);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(Alumno alumno)
        {
            if (ModelState.IsValid)
            {
                if (await GuardarAlumno(alumno,true))
                    return RedirectToAction("Index");
            }
            return View("Index");
        }

        public async Task<ActionResult> Edit(int id)
        {
            Alumno alumno = await ObtenerAlumno(id);
            return View(alumno);
        }
                
        [HttpPost]
        public async Task<ActionResult> Edit(Alumno alumno)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (await GuardarAlumno(alumno,false))
                        return RedirectToAction("Index");
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public async Task<ActionResult> Delete(int id)
        {
            return View(await ObtenerAlumno(id));
        }

        [HttpPost]
        public async Task<ActionResult> Delete(Alumno alumno)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if(await EliminarAlumno(alumno))
                        return RedirectToAction("Index");
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        private async Task<List<Alumno>> ObtenerAlumnos()
        {
            using (var httpClient = new HttpClient())
            {
                var json = await httpClient.GetStringAsync(string.Format("{0}/alumno", uri));
                List<Alumno> alumnos = JsonConvert.DeserializeObject<List<Alumno>>(json).Where(c => c.Estatus == true).Select(c => c).ToList();

                return alumnos;
            }
        }

        private async Task<Alumno> ObtenerAlumno(int id)
        {
            using (var httpClient = new HttpClient())
            {
                var json = await httpClient.GetStringAsync(string.Format("{0}/alumno/{1}", uri, id));
                Alumno alumno = JsonConvert.DeserializeObject<Alumno>(json);

                return alumno;
            }
        }

        private async Task<bool> GuardarAlumno(Alumno alumno, bool nuevo)
        {
            using (var client = new HttpClient())
            {
                var strNJsonInsert = new
                {
                    nombre = alumno.Nombre,
                    apellidos = alumno.Apellidos,
                    telefono = alumno.Telefono,
                    email = alumno.Email,
                    estatus = alumno.Estatus
                };
               
                var strNJsonUpdate = new
                {
                    id = alumno.Id,
                    nombre = alumno.Nombre,
                    apellidos = alumno.Apellidos,
                    telefono = alumno.Telefono,
                    email = alumno.Email,
                    estatus = alumno.Estatus
                };
                
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                string json = (nuevo) ? javaScriptSerializer.Serialize(strNJsonInsert) : javaScriptSerializer.Serialize(strNJsonUpdate);

                client.BaseAddress = new Uri(uri);

                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(string.Format("{0}/alumno", uri), stringContent);

                return response.IsSuccessStatusCode;
            }
        }

        private async Task<bool> EliminarAlumno(Alumno alumno)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(uri);

                var response = await client.DeleteAsync(string.Format("{0}/alumno/{1}", uri, alumno.Id.ToString()));

                return response.IsSuccessStatusCode;
            }
        }
}
}
