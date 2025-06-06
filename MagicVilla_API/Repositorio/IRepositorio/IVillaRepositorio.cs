﻿using MagicVilla_API.Modelos;

namespace MagicVilla_API.Repositorio.IRepositorio
{
    public interface IVillaRepositorio : IRepositorio<Villa>
    {
        //Metodo de actualizacion 
        Task<Villa> Actualizar(Villa entidad);
    }
}
