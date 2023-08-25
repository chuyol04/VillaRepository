using MagicVilla_API.Modelos;
using MagicVilla_API.Modelos.Dto;

namespace MagicVilla_API.Repositorio.IRepositorio
{
    public interface IUsuarioRepositorio
    {
        //si el usuario es el unico y no existe otro con el mismo
        bool IsUsuarioUnico(string userName);

        Task<LoginResponseDto> Login(LoginRequestDto loginRequestDTO);

        Task<UsuarioDto> Registrar(RegistroRequestDto registroRequestDto);


    }
}
