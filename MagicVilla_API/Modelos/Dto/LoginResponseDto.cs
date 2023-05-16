using MagicVillaAPI.Migrations;

namespace MagicVilla_API.Modelos.Dto
{
    public class LoginResponseDto
    {
        public Uusario Usuario { get; set; }

        public string Token { get; set; }   
    }
}
