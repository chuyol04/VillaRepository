using MagicVilla_Utilidad;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;

namespace MagicVilla_Web.Services
{
    public class UsuarioService : BaseService, IUsuarioService
    {

        private readonly IHttpClientFactory _httpClient;
        private string _villaUrl;

        public UsuarioService(IHttpClientFactory httpClient, IConfiguration configuration) :base(httpClient)
        {
            _httpClient = httpClient;
            _villaUrl = configuration.GetValue<string>("ServiceUrls:API_URL");

        }

        public Task<T> Login<T>(LoginRequestDto dto)
        {
            return SendAsync<T>(new APIRequest()
            {
                APITipo = DS.APITipo.POST,
                Datos= dto,
                URL = _villaUrl + "/api/usuario/login"
            });
        }

        public Task<T> Registrar<T>(RegistroRequestDto dto)
        {
            return SendAsync<T>(new APIRequest()
            {
                APITipo = DS.APITipo.POST,
                Datos = dto,
                URL = _villaUrl + "/api/usuario/registrar"
            });
        }
    }
}
