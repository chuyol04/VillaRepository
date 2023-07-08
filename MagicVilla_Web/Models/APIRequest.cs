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

        public Parametros Parametros { get; set; }

    }

    public class Parametros
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }
    }

}
