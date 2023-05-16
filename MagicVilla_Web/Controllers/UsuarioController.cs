using MagicVilla_API.Modelos;
using MagicVilla_Utilidad;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;


namespace MagicVilla_Web.Controllers
{
    public class UsuarioController : Controller
    {

        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequestDto modelo)
        {
            var response = await _usuarioService.Login<APIResponse>(modelo);
            if (response != null && response.IsExitoso == true)
            {
                LoginResponseDto loginResponse = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(response.Resultado));

                //claims para mantrener el user name y rol en todo momento
                //se agregan claims para gaurdar username y rol y agregar serrivios y pipelines
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.Name, loginResponse.Usuario.UserName));
                identity.AddClaim(new Claim(ClaimTypes.Role, loginResponse.Usuario.Rol));
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                //sesiones llenado
                HttpContext.Session.SetString(DS.SessionToken, loginResponse.Token);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                //errores para que el usuario tenga en pantalla los errores
                ModelState.AddModelError("ErrorMessages", response.ErrorMessages.FirstOrDefault());
                return View(modelo);
            }
        }

        public IActionResult Registrar()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar(RegistroRequestDto modelo)
        {
            var response = await _usuarioService.Registrar<APIResponse>(modelo);
            if (response !=null && response.IsExitoso)
            {
                return RedirectToAction("login");
            }
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Session.SetString(DS.SessionToken, "");

            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccesoDenegado()
        {
            return View();
        }
    }
}
