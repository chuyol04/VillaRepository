﻿using MagicVillaAPI.Migrations;

namespace MagicVilla_API.Modelos.Dto
{
    public class LoginResponseDto
    {
        public UsuarioDto Usuario { get; set; }

        public string Token { get; set; }   

        public string Rol { get; set; }
    }
}
