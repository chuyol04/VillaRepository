using MagicVilla_API.Modelos;

namespace MagicVilla_API.Repositorio.IRepositorio
{
    public interface INumeroVillaRepositorio : IRepositorio<NumeroVilla>
    {
        //Metodo de actualizacion 
        Task<NumeroVilla> Actualizar(NumeroVilla entidad);
    }
}
