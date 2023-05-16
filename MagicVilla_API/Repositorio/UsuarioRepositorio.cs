using MagicVilla_API.Datos;
using MagicVilla_API.Modelos;
using MagicVilla_API.Modelos.Dto;
using MagicVilla_API.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MagicVilla_API.Repositorio
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly ApplicationDbContext _db;
        //token
        private string secretKey;

        public UsuarioRepositorio(ApplicationDbContext db, IConfiguration configuration)
        {
            _db = db;
            //token desde appsettingkjson
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
        }


        public bool IsUsuarioUnico(string userName)
        {
            var usuario = _db.Usarios.FirstOrDefault(u => u.UserName.ToLower() == userName.ToLower());
            if (usuario == null)
            {
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDTO)
        {
            //validar si el usuario que se desea registrar exsite en la BD
            var usuario = await _db.Usarios.FirstOrDefaultAsync(u => u.UserName.ToLower() == loginRequestDTO.UserName.ToLower() && 
                                                                u.Password == loginRequestDTO.Password);
            if (usuario == null)
            {
                //si es null 
                return new LoginResponseDto()
                {
                    Token = "",
                    Usuario = null
                };
            }

            //Si usuario existe generamos el TOKEN el jw token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, usuario.Id.ToString()),
                    new Claim(ClaimTypes.Role, usuario.Rol)
                }),
                //tiempo que dura el toke, le puse 2 dias
                Expires = DateTime.UtcNow.AddDays(2),
                SigningCredentials = new (new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            //se comienza a llenar con el token y usuario
            LoginResponseDto loginResponseDTO = new()
            {
                Token = tokenHandler.WriteToken(token),
                Usuario = usuario
            };

            return loginResponseDTO;

        }

        public async Task<Uusario> Registrar(RegistroRequestDto registroRequestDto)
        {
            Uusario usuario = new()
            {
                UserName = registroRequestDto.UserName,
                Password = registroRequestDto.Password,
                Nombres = registroRequestDto.Nombres,
                Rol = registroRequestDto.Rol

            };
            await _db.Usarios.AddAsync(usuario);
            await _db.SaveChangesAsync();
            usuario.Password = "";
            return usuario;
        }
    }
}
