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
    public class MateriaController : Controller
    {
        public String uri = System.Configuration.ConfigurationManager.ConnectionStrings["APIEscolar"].ToString();

        public async Task<ActionResult> Index()
        {
            List<Materia> listaMaterias = await ObtenerMaterias();
            return View(listaMaterias);
        }

        public async Task<ActionResult> Details(int id)
        {
            Materia materia = await ObtenerMateria(id);
            return View(materia);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(Materia materia)
        {
            if (ModelState.IsValid)
            {
                if (await GuardarMateria(materia, true))
                    return RedirectToAction("Index");
            }
            return View("Index");
        }

        public async Task<ActionResult> Edit(int id)
        {
            Materia materia = await ObtenerMateria(id);
            return View(materia);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(Materia materia)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (await GuardarMateria(materia, false))
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
            return View(await ObtenerMateria(id));
        }

        [HttpPost]
        public async Task<ActionResult> Delete(Materia materia)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (await EliminarMateria(materia))
                        return RedirectToAction("Index");
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public async Task<List<Materia>> ObtenerMaterias()
        {
            using (var httpClient = new HttpClient())
            {
                var json = await httpClient.GetStringAsync(string.Format("{0}/materia", uri));
                List<Materia> materias = JsonConvert.DeserializeObject<List<Materia>>(json).Where(c => c.Estatus == true).Select(c => c).ToList();

                return materias;
            }
        }

        public async Task<Materia> ObtenerMateria(int id)
        {
            using (var httpClient = new HttpClient())
            {
                var json = await httpClient.GetStringAsync(string.Format("{0}/materia/{1}", uri, id));
                Materia materia = JsonConvert.DeserializeObject<Materia>(json);

                return materia;
            }
        }

        private async Task<bool> GuardarMateria(Materia materia, bool nuevo)
        {
            using (var client = new HttpClient())
            {
                var strNJsonInsert = new
                {
                    nombre = materia.Nombre,
                    estatus = materia.Estatus
                };

                var strNJsonUpdate = new
                {
                    id = materia.Id,
                    nombre = materia.Nombre,
                    estatus = materia.Estatus
                };

                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                string json = (nuevo) ? javaScriptSerializer.Serialize(strNJsonInsert) : javaScriptSerializer.Serialize(strNJsonUpdate);

                client.BaseAddress = new Uri(uri);

                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(string.Format("{0}/materia", uri), stringContent);

                return response.IsSuccessStatusCode;
            }
        }

        private async Task<bool> EliminarMateria(Materia materia)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(uri);

                var response = await client.DeleteAsync(string.Format("{0}/materia/{1}", uri, materia.Id.ToString()));

                return response.IsSuccessStatusCode;
            }
        }
    }
}
