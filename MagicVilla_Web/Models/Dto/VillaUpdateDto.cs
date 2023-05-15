using System.ComponentModel.DataAnnotations;

namespace MagicVilla_Web.Models.Dto
{
    //temas de presentacion para no usar nuestro odelo principal que es villa.cs
    public class VillaUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Nombre { get; set; }

        public string Detalle { get; set; }

        [Required]
        public double Tarifa { get; set; }

        [Required]
        public int Ocupantes { get; set; }

        [Required]
        public int MetrosCuadrados { get; set; }

        [Required]
        public string ImagenUrl { get; set; }

        public string Amenidad { get; set; }

        // public DateTime fechaCreacion { get; set; }
    }
}
