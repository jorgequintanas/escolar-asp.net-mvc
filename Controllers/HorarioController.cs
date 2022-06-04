using AplicacionWebEscolar.Models;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
    public class HorarioController : Controller
    {
        public String uri = System.Configuration.ConfigurationManager.ConnectionStrings["APIEscolar"].ToString();
        
        public async Task<ActionResult> Index()
        {
            List<Horario> listaHorarios = await ObtenerHorarios();
            return View(listaHorarios);
        }

        public async Task<ActionResult> Details(int id)
        {
            Horario horario = await ObtenerHorario(id);
            horario.Detalles = await ObtenerDetalles(horario);
            return View(horario);
        }

        public ActionResult Create()
        {
            Horario horario = new Horario();
            
            ViewBag.Dias = listadoItems("dias");
            ViewBag.Horas = listadoItems("horas");

            return View(horario);
        }
                
        [HttpPost]
        public async Task<ActionResult> Create(Horario modelo, string operacion = null)
        {
            if (modelo == null)
            {
                modelo = new Horario();
            }

            if (operacion == null)
            {
                if (await CrearHorario(modelo))
                {
                    return RedirectToAction("Index");
                }
            }
            else if (operacion == "agregar-detalle")
            {
                modelo.Detalles.Add(new HorarioDetalle());
            }
            else if (operacion.StartsWith("eliminar-detalle-"))
            {
                EliminarDetallePorIndice(modelo, operacion);
            }

            ViewBag.Dias = listadoItems("dias");
            ViewBag.Horas = listadoItems("horas");
            
            return View(modelo);
        }

        public async Task<ActionResult> Edit(int id)
        {
            Horario horario = await ObtenerHorario(id);
            
            ViewBag.Dias = listadoItems("dias");
            ViewBag.Horas = listadoItems("horas");
            
            return View(horario);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(Horario modelo, string operacion = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (operacion == null)
                    {
                        if (await CrearHorario(modelo))
                        {
                            return RedirectToAction("Index");
                        }
                    }
                    else if (operacion == "agregar-detalle")
                    {
                        modelo.Detalles.Add(new HorarioDetalle());
                    }
                    else if (operacion.StartsWith("eliminar-detalle-"))
                    {
                        EliminarDetallePorIndice(modelo, operacion);
                    }

                    ViewBag.Dias = listadoItems("dias");
                    ViewBag.Horas = listadoItems("horas");

                    return View(modelo);
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
            return View(await ObtenerHorario(id));
        }

        [HttpPost]
        public async Task<ActionResult> Delete(Horario horario)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (await EliminarHorario(horario))
                        return RedirectToAction("Index");
                }
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        private SelectList listadoItems(String listado)
        {
            List<SelectListItem> lst = new List<SelectListItem>();

            switch (listado)
            {
                case "dias":
                    lst.Add(new SelectListItem() { Text = "Seleccione", Value = "" });
                    lst.Add(new SelectListItem() { Text = "Lunes", Value = "1" });
                    lst.Add(new SelectListItem() { Text = "Martes", Value = "2" });
                    lst.Add(new SelectListItem() { Text = "Miércoles", Value = "3" });
                    lst.Add(new SelectListItem() { Text = "Jueves", Value = "4" });
                    lst.Add(new SelectListItem() { Text = "Viernes", Value = "5" });
                    lst.Add(new SelectListItem() { Text = "Sábado", Value = "6" });
                    lst.Add(new SelectListItem() { Text = "Domingo", Value = "7" });
                    break;
                case "horas":
                    lst.Add(new SelectListItem() { Text = "Seleccione", Value = "" });
                    lst.Add(new SelectListItem() { Text = "07:00:00", Value = "07:00:00" });
                    lst.Add(new SelectListItem() { Text = "08:00:00", Value = "08:00:00" });
                    lst.Add(new SelectListItem() { Text = "09:00:00", Value = "09:00:00" });
                    lst.Add(new SelectListItem() { Text = "10:00:00", Value = "10:00:00" });
                    lst.Add(new SelectListItem() { Text = "11:00:00", Value = "11:00:00" });
                    lst.Add(new SelectListItem() { Text = "12:00:00", Value = "12:00:00" });
                    lst.Add(new SelectListItem() { Text = "13:00:00", Value = "13:00:00" });
                    lst.Add(new SelectListItem() { Text = "14:00:00", Value = "14:00:00" });
                    lst.Add(new SelectListItem() { Text = "15:00:00", Value = "15:00:00" });
                    lst.Add(new SelectListItem() { Text = "16:00:00", Value = "16:00:00" });
                    lst.Add(new SelectListItem() { Text = "17:00:00", Value = "17:00:00" });
                    lst.Add(new SelectListItem() { Text = "18:00:00", Value = "18:00:00" });
                    lst.Add(new SelectListItem() { Text = "19:00:00", Value = "19:00:00" });
                    lst.Add(new SelectListItem() { Text = "20:00:00", Value = "20:00:00" });
                    break;
            }

            return new SelectList(lst, "Value", "Text");
        }

        public async Task<List<Horario>> ObtenerHorarios()
        {
            using (var httpClient = new HttpClient())
            {
                var json = await httpClient.GetStringAsync(string.Format("{0}/horario", uri));
                List<Horario> horarios = JsonConvert.DeserializeObject<List<Horario>>(json).Where(c => c.Estatus == true).Select(c => c).ToList();

                return horarios;
            }
        }

        public async Task<Horario> ObtenerHorario(int id)
        {
            using (var httpClient = new HttpClient())
            {
                var json = await httpClient.GetStringAsync(string.Format("{0}/horario/{1}", uri, id));
                Horario horario = JsonConvert.DeserializeObject<Horario>(json);

                var jsonD = await httpClient.GetStringAsync(string.Format("{0}/horariodetalle", uri));
                List<HorarioDetalle> detalles = JsonConvert.DeserializeObject<List<HorarioDetalle>>(jsonD).Where(c => c.Horario.Id == id).Select(c => c).ToList();

                horario.Detalles = await ObtenerDetalles(horario);

                return horario;
            }
        }

        public async Task<List<HorarioDetalle>> ObtenerDetalles(Horario horario)
        {
            using (var httpClient = new HttpClient())
            {
                var jsonD = await httpClient.GetStringAsync(string.Format("{0}/horariodetalle", uri));
                List<HorarioDetalle> detalles = JsonConvert.DeserializeObject<List<HorarioDetalle>>(jsonD).Where(c => c.Horario.Id == horario.Id).Select(c => c).ToList();

                return detalles;
            }
        }

        private async Task<bool> CrearHorario(Horario horario)
        {
            if (ModelState.IsValid)
            {
                if (horario.Detalles != null && horario.Detalles.Count > 0)
                {
                    return await GuardarHorario(horario, true);
                }
                else
                {
                    ModelState.AddModelError("", "No puede guardar horario sin detalle");
                }
            }
            return false;
        }

        private async Task<bool> GuardarHorario(Horario horario, bool nuevo)
        {
            using (var client = new HttpClient())
            {
                var strNJsonInsert = new
                {
                    nombre = horario.Nombre,
                    estatus = horario.Estatus
                };

                var strNJsonUpdate = new
                {
                    id = horario.Id,
                    nombre = horario.Nombre,
                    estatus = horario.Estatus
                };

                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                string json = (nuevo) ? javaScriptSerializer.Serialize(strNJsonInsert) : javaScriptSerializer.Serialize(strNJsonUpdate);

                client.BaseAddress = new Uri(uri);

                WebRequest oRequest = WebRequest.Create(string.Format("{0}/horario", uri));
                oRequest.Method = "POST";
                oRequest.ContentType = "application/json;charset=UTF-8";

                using (var oSW = new StreamWriter(oRequest.GetRequestStream()))
                {
                    oSW.Write(json);
                    oSW.Flush();
                    oSW.Close();
                }

                WebResponse oResponse = oRequest.GetResponse();
                Horario h = new Horario();

                using (var oSR = new StreamReader(oResponse.GetResponseStream()))
                {
                    try
                    {
                        h = JsonConvert.DeserializeObject<Horario>(oSR.ReadToEnd().Trim());
                    }
                    catch (Exception ex) { }
                }
                if (h != null)
                {
                    if (!nuevo)
                        if (!await EliminarDetallesHorario(horario))
                            return false;
                    
                    bool resultado = false;

                    foreach (HorarioDetalle detalle in horario.Detalles)
                    {
                        detalle.Horario = new Horario();
                        detalle.Horario.Id = h.Id;
                        detalle.Horario.Nombre = h.Nombre;
                        detalle.Horario.Estatus = h.Estatus;

                        var strNJsonInsertD = new
                        {
                            diaSemana = detalle.diaSemana,
                            horaInicio = detalle.horaInicio,
                            horaFin = detalle.horaFin,
                            horario = new
                            {
                                id = h.Id,
                                nombre = h.Nombre,
                                estatus = h.Estatus
                            }
                        };

                        var strNJsonUpdateD = new
                        {
                            id = horario.Id,
                            diaSemana = detalle.diaSemana,
                            horaInicio = detalle.horaInicio,
                            horaFin = detalle.horaFin,
                            horario = new
                            {
                                id = h.Id,
                                nombre = h.Nombre,
                                estatus = h.Estatus
                            }
                        };

                        JavaScriptSerializer javaScriptSerializerD = new JavaScriptSerializer();
                        string jsonD = (nuevo) ? javaScriptSerializerD.Serialize(strNJsonInsertD) : javaScriptSerializerD.Serialize(strNJsonUpdateD);

                        var stringContentD = new StringContent(jsonD, Encoding.UTF8, "application/json");
                        var responseD = await client.PostAsync(string.Format("{0}/horariodetalle", uri), stringContentD);
                  
                        resultado =  responseD.IsSuccessStatusCode;
                    }
                   return resultado;
                }
                return false;
            }

        }

        private async Task<bool> EliminarHorario(Horario horario)
        {
            horario.Detalles = await ObtenerDetalles(horario);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(uri);

                foreach (HorarioDetalle detalle in horario.Detalles)
                {
                    var responseD = await client.DeleteAsync(string.Format("{0}/horariodetalle/{1}", uri, detalle.Id.ToString()));

                    if (!responseD.IsSuccessStatusCode)
                        return false;
                }

                var response = await client.DeleteAsync(string.Format("{0}/horario/{1}", uri, horario.Id.ToString()));

                return response.IsSuccessStatusCode;
            }
        }

        private async Task<bool> EliminarDetallesHorario(Horario horario)
        {
            horario.Detalles = await ObtenerDetalles(horario);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(uri);

                foreach (HorarioDetalle detalle in horario.Detalles)
                {
                    var response = await client.DeleteAsync(string.Format("{0}/horariodetalle/{1}", uri, detalle.Id.ToString()));

                    if (!response.IsSuccessStatusCode)
                        return false;
                }

                return true;
            }
        }

        private static void EliminarDetallePorIndice(Horario horario, string operacion)
        {
            string indexStr = operacion.Replace("eliminar-detalle-", "");
            int index = 0;

            if (int.TryParse(indexStr, out index) && index >= 0 && index < horario.Detalles.Count)
            {
                var item = horario.Detalles.ToArray()[index];
                horario.Detalles.Remove(item);
            }
        }
    }
}
