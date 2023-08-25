using MagicVilla_API.Modelos;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace MagicVilla_API.Datos
{
    public class ApplicationDbContext : IdentityDbContext<UsuarioAplicacion>
    {
        //constructor. base es de donde hereda. 
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :base(options)
        {
            
        }

        //se crea tabla que estara asociada
        public DbSet<UsuarioAplicacion> UsuariosAplicacion { get; set; }

        //se crea tablas en BD
        public DbSet<Villa> Villas { get; set; }

        public DbSet<Uusario> Usarios { get; set; }

        //migracion para tabla en BD 
        public DbSet<NumeroVilla> NumeroVillas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //para que todo se relacione y no haya tema con primary key con el identity
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Villa>().HasData(
                new Villa()
                {
                    Id = 1,
                    Nombre = "Villa Real",
                    Detalle = "Detalle de la Villa...",
                    ImagenUrl = "",
                    Ocupantes = 5,
                    MetrosCuadrados = 50,
                    Tarifa = 200,
                    Amenidad = "",
                    FechaCreacion = DateTime.Now,
                    FechaActualizacion = DateTime.Now
                },
                   new Villa()
                   {
                       Id = 2,
                       Nombre = "Premium Vista a la piscina",
                       Detalle = "Detalle de la Villa...",
                       ImagenUrl = "",
                       Ocupantes = 4,
                       MetrosCuadrados = 40,
                       Tarifa = 150,
                       Amenidad = "",
                       FechaCreacion = DateTime.Now,
                       FechaActualizacion = DateTime.Now
                   }
            );
        }

    }
}
