using AutoMapper;
using MagicVilla_API.Modelos;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Models.ViewModel;
using MagicVilla_Web.Services;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace MagicVilla_Web.Controllers
{
    public class NumeroVillaController : Controller
    {

        private readonly INumeroVillaService _numerovillaService;
        private readonly IMapper _mapper;
        private readonly IVillaService _villaService;

        public NumeroVillaController(INumeroVillaService numerovillaService, IVillaService villaService, IMapper mapper)
        {
            _numerovillaService = numerovillaService;
            _villaService = villaService;
            _mapper = mapper;
        }

       

        public async Task <IActionResult> IndexnumeroVilla()
        {
            List<NumeroVillaDto> numerovillaList = new();

            var response = await _numerovillaService.ObtenerTodos<APIResponse>();

            if (response != null && response.IsExitoso)
            {
                numerovillaList = JsonConvert.DeserializeObject<List<NumeroVillaDto>>(Convert.ToString(response.Resultado));
            }

            return View(numerovillaList);
        }

        public async Task<IActionResult> CrearNumeroVilla()
        {
            NumeroVillaViewModel numeroVillaVM = new();

            var response = await _villaService.ObtenerTodos<APIResponse>();

            if (response != null && response.IsExitoso)
            {
                numeroVillaVM.VillaList = JsonConvert.DeserializeObject<List<VillaDto>>(Convert.ToString(response.Resultado))
                                          .Select(v => new SelectListItem
                                          {
                                              Text = v.Nombre,
                                              Value = v.Id.ToString()
                                          });
            }

            return View(numeroVillaVM);
        }


        //MetodoPost para hacer la operacion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearNumeroVilla(NumeroVillaViewModel modelo)
        {
            if (ModelState.IsValid)
            {
                var response = await _numerovillaService.Crear<APIResponse>(modelo.NumeroVilla);
                if (response != null && response.IsExitoso)
                {
                    return RedirectToAction(nameof(IndexnumeroVilla));
                }
                else
                {
                    if (response.ErrorMessages.Count>0)
                    {
                        ModelState.AddModelError("ErrorMessages", response.ErrorMessages.FirstOrDefault());
                    }
                }
            }

            var res = await _villaService.ObtenerTodos<APIResponse>();

            if (res != null && res.IsExitoso)
            {
                modelo.VillaList = JsonConvert.DeserializeObject<List<VillaDto>>(Convert.ToString(res.Resultado))
                                          .Select(v => new SelectListItem
                                          {
                                              Text = v.Nombre,
                                              Value = v.Id.ToString()
                                          });
            }
            return View(modelo);
        }

        //deeb se exacto el parametro que tengo en el viws de numero vila villaNo
        //metodo get llamada
        public async Task <IActionResult> ActualizarNumeroVilla(int villaNo)
        {
            NumeroVillaUpdateViewModel numeroVillaVM = new();

            var response = await _numerovillaService.Obtener<APIResponse>(villaNo);

            if (response  != null && response.IsExitoso)
            {
                NumeroVillaDto modelo = JsonConvert.DeserializeObject<NumeroVillaDto>(Convert.ToString(response.Resultado));
                numeroVillaVM.NumeroVilla = _mapper.Map<NumeroVillaUpdateDto>(modelo);
            }

            response = await _villaService.ObtenerTodos<APIResponse>();

            if (response != null && response.IsExitoso)
            {
                numeroVillaVM.VillaList = JsonConvert.DeserializeObject<List<VillaDto>>(Convert.ToString(response.Resultado))
                                          .Select(v => new SelectListItem
                                          {
                                              Text = v.Nombre,
                                              Value = v.Id.ToString()
                                          });
                return View(numeroVillaVM);
            }

            return NotFound();

        }

        //este realiza la operacion de actualizar
        [HttpPost]
        public async Task<IActionResult> ActualizarNumeroVilla(NumeroVillaUpdateViewModel modelo)
        {
            if (ModelState.IsValid)
            {
                var response = await _numerovillaService.Actualizar<APIResponse>(modelo.NumeroVilla);
                if (response != null && response.IsExitoso)
                {
                    return RedirectToAction(nameof(IndexnumeroVilla));
                }
                else
                {
                    if (response.ErrorMessages.Count > 0)
                    {
                        ModelState.AddModelError("ErrorMessages", response.ErrorMessages.FirstOrDefault());
                    }
                }
            }

            var res = await _villaService.ObtenerTodos<APIResponse>();

            if (res != null && res.IsExitoso)
            {
                modelo.VillaList = JsonConvert.DeserializeObject<List<VillaDto>>(Convert.ToString(res.Resultado))
                                          .Select(v => new SelectListItem
                                          {
                                              Text = v.Nombre,
                                              Value = v.Id.ToString()
                                          });
            }
            return View(modelo);
        }



        //metodo get llamada
        public async Task<IActionResult> RemoverNumeroVilla(int villaNo)
        {
            NumeroVillaDeleteViewModel numeroVillaVM = new();

            var response = await _numerovillaService.Obtener<APIResponse>(villaNo);

            if (response != null && response.IsExitoso)
            {
                NumeroVillaDto modelo = JsonConvert.DeserializeObject<NumeroVillaDto>(Convert.ToString(response.Resultado));
                numeroVillaVM.NumeroVilla = modelo;
            }

            response = await _villaService.ObtenerTodos<APIResponse>();

            if (response != null && response.IsExitoso)
            {
                numeroVillaVM.VillaList = JsonConvert.DeserializeObject<List<VillaDto>>(Convert.ToString(response.Resultado))
                                          .Select(v => new SelectListItem
                                          {
                                              Text = v.Nombre,
                                              Value = v.Id.ToString()
                                          });
                return View(numeroVillaVM);
            }

            return NotFound();

        }

        //este realiza la operacion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoverNumeroVilla(NumeroVillaDeleteViewModel modelo)
        {
            var response = await _numerovillaService.Remover<APIResponse>(modelo.NumeroVilla.VillaNo);
            if (response != null && response.IsExitoso)
            {
                return RedirectToAction(nameof(IndexnumeroVilla));
            }
            return View(modelo);
        }

    }
}
