using System.Linq.Expressions;

namespace MagicVilla_API.Repositorio.IRepositorio
{
    public interface IRepositorio<T> where T : class
    {
        Task Crear(T entidad);

        /*devolver lista de objetos de tipo T llamado Obtener todos y toma el parametro opcional filtro de tipo expression.
         *de manera asincrona que cumplen con una condicion especificada */
        Task<List<T>> ObtenerTodos(Expression<Func<T, bool>>? filtro = null);

        //"tracked" de tipo bool que se utiliza para indicar si el objeto devuelto debe ser rastreado
        //por el contexto de Entity Framework o no.
        Task<T> Obtener(Expression<Func<T, bool>> filtro = null, bool tracked = true);

        Task Remover(T entidad);

        Task Grabar();


    }
}
