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
    public class MaestroController : Controller
    {
        public String uri = System.Configuration.ConfigurationManager.ConnectionStrings["APIEscolar"].ToString();

        public async Task<ActionResult> Index()
        {
            List<Maestro> listaMaestros = await ObtenerMaestros();
            return View(listaMaestros);
        }

        public async Task<ActionResult> Details(int id)
        {
            Maestro maestro = await ObtenerMaestro(id);
            return View(maestro);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(Maestro maestro)
        {
            if (ModelState.IsValid)
            {
                if (await GuardarMaestro(maestro, true))
                    return RedirectToAction("Index");
            }
            return View("Index");
        }

        public async Task<ActionResult> Edit(int id)
        {
            Maestro maestro = await ObtenerMaestro(id);
            return View(maestro);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(Maestro maestro)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (await GuardarMaestro(maestro, false))
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
            return View(await ObtenerMaestro(id));
        }

        [HttpPost]
        public async Task<ActionResult> Delete(Maestro maestro)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (await EliminarMaestro(maestro))
                        return RedirectToAction("Index");
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public async Task<List<Maestro>> ObtenerMaestros()
        {
            using (var httpClient = new HttpClient())
            {
                var json = await httpClient.GetStringAsync(string.Format("{0}/maestro", uri));
                List<Maestro> maestros = JsonConvert.DeserializeObject<List<Maestro>>(json).Where(c => c.Estatus == true).Select(c => c).ToList();

                return maestros;
            }
        }

        public async Task<Maestro> ObtenerMaestro(int id)
        {
            using (var httpClient = new HttpClient())
            {
                var json = await httpClient.GetStringAsync(string.Format("{0}/maestro/{1}", uri, id));
                Maestro maestro = JsonConvert.DeserializeObject<Maestro>(json);

                return maestro;
            }
        }

        private async Task<bool> GuardarMaestro(Maestro maestro, bool nuevo)
        {
            using (var client = new HttpClient())
            {
                var strNJsonInsert = new
                {
                    nombre = maestro.Nombre,
                    apellidos = maestro.Apellidos,
                    telefono = maestro.Telefono,
                    email = maestro.Email,
                    estatus = maestro.Estatus
                };

                var strNJsonUpdate = new
                {
                    id = maestro.Id,
                    nombre = maestro.Nombre,
                    apellidos = maestro.Apellidos,
                    telefono = maestro.Telefono,
                    email = maestro.Email,
                    estatus = maestro.Estatus
                };

                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                string json = (nuevo) ? javaScriptSerializer.Serialize(strNJsonInsert) : javaScriptSerializer.Serialize(strNJsonUpdate);

                client.BaseAddress = new Uri(uri);

                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(string.Format("{0}/maestro", uri), stringContent);

                return response.IsSuccessStatusCode;
            }
        }

        private async Task<bool> EliminarMaestro(Maestro maestro)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(uri);

                var response = await client.DeleteAsync(string.Format("{0}/maestro/{1}", uri, maestro.Id.ToString()));

                return response.IsSuccessStatusCode;
            }
        }
    }
}
