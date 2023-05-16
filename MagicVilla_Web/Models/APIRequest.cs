using static MagicVilla_Utilidad.DS;

namespace MagicVilla_Web.Models
{
    public class APIRequest
    {
        //necesarias para las primeras solicitudes.
        public APITipo APITipo { get; set; } = APITipo.GET;

        public string URL { get; set; } 

        public object Datos { get; set; }

        public string Token { get; set; }

    }
}
