using AutoMapper;
using MagicVilla_API.Modelos;
using MagicVilla_Utilidad;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using Twilio;
using Twilio.Rest.Api.V2010.Account;


namespace MagicVilla_Web.Controllers
{
    public class VillaController : Controller
    {

        private readonly IVillaService _villaService;
        private readonly IMapper _mapper;

        public VillaController(IVillaService villaService, IMapper mapper)
        {
            _villaService = villaService;
            _mapper = mapper;
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> IndexVilla()
        {
            List<VillaDto> villaList = new();
            var response = await _villaService.ObtenerTodos<APIResponse>(HttpContext.Session.GetString(DS.SessionToken));

            if (response != null && response.IsExitoso)
            {
                villaList = JsonConvert.DeserializeObject<List<VillaDto>>(Convert.ToString(response.Resultado));
            }

            return View(villaList);
        }


        //////////////////////////////////CREAR////////////CREAR/////////////////////////CREAR///////////////************************************************************



        //Get llama a la vista
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CrearVilla()
        {
            return View();
        }

        /*Codigo proyecto Jenny Crear
        //Enviar los datos con el post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearVilla(VillaCreateDto modelo)
        {
            //si estan todos los campos llenos o algo falla esto lo previene
            if(ModelState.IsValid)
            {
                var response = await _villaService.Crear<APIResponse>(modelo);

                if (response !=null && response.IsExitoso)
                {
                    // Obtener la hora actual
                    DateTime horaActual = DateTime.Now;

                    // Convertir la hora actual a una cadena en formato corto de hora
                    string horaActualString = horaActual.ToShortTimeString();


                    // Aquí es donde se enviaría el mensaje de texto utilizando Twilio
                    var accountSid = "AC3cb001583be6dd3d8660978a22405dc6";
                    var authToken = "aa7c2b2d8697dff534cb9de3f2a6b854";
                    TwilioClient.Init(accountSid, authToken);

                    var message = MessageResource.Create(
                        body: "Se ha insertado una nueva Localidad llamada " + modelo.Nombre + " a las: " + horaActualString,
                        from: new Twilio.Types.PhoneNumber("+12705696305"),
                        to: new Twilio.Types.PhoneNumber("+528123707085")
                    );

                    
                 return RedirectToAction(nameof(IndexVilla));
                    
                }
            }
            return View(modelo);
        }

        */
        //Enviar los datos con el post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearVilla(VillaCreateDto modelo)
        {
            //si estan todos los campos llenos o algo falla esto lo previene
            if (ModelState.IsValid)
            {
                var response = await _villaService.Crear<APIResponse>(modelo, HttpContext.Session.GetString(DS.SessionToken));

                if (response != null && response.IsExitoso)
                {
                    return RedirectToAction(nameof(IndexVilla));
                }
            }
            return View(modelo);
        }

        ///////////actualizar///////////////////////////actualizar////////////////Actualizar////////////////////////////////************************************************************



        //Este llama a la vista 
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> ActualizarVilla(int villaId)
        {
            var response = await _villaService.Obtener<APIResponse>(villaId, HttpContext.Session.GetString(DS.SessionToken));

            if (response != null && response.IsExitoso)
            {
                VillaDto model = JsonConvert.DeserializeObject<VillaDto>(Convert.ToString(response.Resultado));
                return View(_mapper.Map<VillaUpdateDto>(model));
            }

            return NotFound();
        }

        //este se encarga de actualizar los registros 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarVilla(VillaUpdateDto modelo)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaService.Actualizar<APIResponse>(modelo, HttpContext.Session.GetString(DS.SessionToken));

                if (response != null && response.IsExitoso)
                {
                    return RedirectToAction(nameof(IndexVilla));
                }
            }

            return View(modelo);

        }

        /*PROYECTO JENNY
        //este se encarga de actualizar los registros 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarVilla(VillaUpdateDto modelo)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaService.Actualizar<APIResponse>(modelo);

                if (response != null && response.IsExitoso)
                {
                    // Aquí es donde se enviaría el mensaje de texto utilizando Twilio
                    var accountSid = "AC3cb001583be6dd3d8660978a22405dc6";
                    var authToken = "aa7c2b2d8697dff534cb9de3f2a6b854";
                    TwilioClient.Init(accountSid, authToken);

                    var message = MessageResource.Create(
                        body: "Se ha modificado la Localidad: " + modelo.Nombre,
                        from: new Twilio.Types.PhoneNumber("+12705696305"),
                        to: new Twilio.Types.PhoneNumber("+528123707085")
                    );

                    return RedirectToAction(nameof(IndexVilla));
                }
            }

            return View(modelo);

        }
        /*


        ///remover/***************////////////remover/////////remover///////*********************************///////////////////////////////////////////


        //Este llama a la vista 
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> RemoverVilla(int villaId)
        {
            var response = await _villaService.Obtener<APIResponse>(villaId, HttpContext.Session.GetString(DS.SessionToken));

            if (response != null && response.IsExitoso)
            {
                VillaDto model = JsonConvert.DeserializeObject<VillaDto>(Convert.ToString(response.Resultado));
                return View(model);
            }

            return NotFound();
        }

         //ESTE ES EL DEL PROYECTO ORIGINAL
        //este se encarga de actualizar los registros, quien hace la eliminacion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoverVilla(VillaDto modelo)
        {

            var response = await _villaService.Remover<APIResponse>(modelo.Id, HttpContext.Session.GetString(DS.SessionToken));

            if (response != null && response.IsExitoso)
            {
              
                return RedirectToAction(nameof(IndexVilla));
            }

            return View(modelo);

        }
       

        /*proyecto Jennt remover
        //este se encarga de actualizar los registros, quien hace la eliminacion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoverVilla(VillaDto modelo)
        {
            // Obtener los detalles del modelo antes de eliminarlo
            var responseDetalles = await _villaService.Obtener<APIResponse>(modelo.Id);
            string detalles = "";

            if (responseDetalles != null && responseDetalles.IsExitoso)
            {
                VillaDto detallesModel = JsonConvert.DeserializeObject<VillaDto>(Convert.ToString(responseDetalles.Resultado));
                // Concatenar los detalles relevantes al mensaje
                detalles += "ID: " + detallesModel.Id + ", ";
                detalles += "Nombre: " + detallesModel.Nombre;
            }

            var response = await _villaService.Remover<APIResponse>(modelo.Id);

            if (response != null && response.IsExitoso)
            {
                // Aquí es donde se enviaría el mensaje de texto utilizando Twilio
                var accountSid = "AC3cb001583be6dd3d8660978a22405dc6";
                var authToken = "aa7c2b2d8697dff534cb9de3f2a6b854";
                TwilioClient.Init(accountSid, authToken);

                var message = MessageResource.Create(
                    body: "Se ha eliminado la Localidad. Detalles: " + detalles,
                    from: new Twilio.Types.PhoneNumber("+12705696305"),
                    to: new Twilio.Types.PhoneNumber("+528123707085")
                );

                return RedirectToAction(nameof(IndexVilla));
            }

            return View(modelo);
        }
        */

    }
}
