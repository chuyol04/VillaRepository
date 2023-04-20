using AutoMapper;
using MagicVilla_API.Datos;
using MagicVilla_API.Modelos;
using MagicVilla_API.Modelos.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaController : ControllerBase
    {
        //Crear variable privada. Variables privadas con guiones bajo
        private readonly ILogger<VillaController> _logger;

        private readonly ApplicationDbContext _db;

        private readonly IMapper _mapper;

        //constructor donde pondremos servicio logger //inyectamos dependencias para poder inyectarlo y usarlo
        public VillaController(ILogger<VillaController> logger, ApplicationDbContext db, IMapper mapper)
        {
            //se registra logger
            _logger = logger;
            _db = db;
            _mapper = mapper;
        }


        //1- Endpoint que regresa toda la lista dividido de 2 maneras, uno donde obtiene de todo el modelo
        //y otro donde lo obtenemos de un store
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]//codigos de estado
        public async Task< ActionResult< IEnumerable<VillaDto>>> GetVillas()
        {
            /*
            return new List<VillaDto>
            {  
                //Podemos tomar datos ficticios de esta manera, ya cuando haya BD los tomamos de ahí.
                new VillaDto{Id = 1, Nombre = "Vista a la picina"},
                new VillaDto{Id = 2, Nombre = "Vista a la playa"}
            };
            */


            _logger.LogInformation("Obtener las villas");
            //Accedes del store de la clase llamada Villa Store de la carpeta Datos
            //return Ok( VillaStore.villaList);
            //Sincrono
            //return Ok(_db.Villas.ToList());  ya lo podemos usar por la inyeccion de dependencias, es como un select de la tabla villa

            IEnumerable<Villa> villaList = await _db.Villas.ToListAsync();

            //asincrono en entity framework.
            return Ok(_mapper.Map<IEnumerable<VillaDto>>(villaList));

        }


        //2- Obtenemos la informacion del Store pero solo un valor determinado, no una lista como en el 1.
        //Hay que diferenciar los endpionts ya que no se pueden tener 2 mismos del HTTPGET, se diferencia poniendo en su atributo
        //que en este caso será el ID
        [HttpGet("id:int", Name = "GetVilla")] //Le asignamos nombre
        [ProducesResponseType(StatusCodes.Status200OK)]//codigos de estado
        [ProducesResponseType(StatusCodes.Status400BadRequest)]//codigos de estado
        [ProducesResponseType(StatusCodes.Status404NotFound)]//codigos de estado
        public async Task <ActionResult<VillaDto>> GetVilla(int id)
        {

            if (id == 0)
            {
                _logger.LogError("Error al traer villa con Id " + id);
                //Mala solicitud, codigo de estado 400
                return BadRequest();
            }
            //se guarde en una variable en base al id para que guarde el registro
            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            //var villa =  _db.Villas.FirstOrDefault(v => v.Id == id);

            //metodo asincrono
            var villa = await _db.Villas.FirstOrDefaultAsync(v => v.Id == id);

            //villa sea nullo
            if (villa == null)
            {
                //no encontro ningun registro error 404
                return NotFound();
            }


            //return Ok(villa); ANTES

            //Este solo retorna un solo objeto en base al ID, codigo 200 OK
            return Ok(_mapper.Map<VillaDto>(villa));
        }

        [HttpPost]//Crear
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] //Tamaño maximo del parametro
        public async Task<ActionResult<VillaDto>> CrearVilla([FromBody] VillaCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                //si alguna de las propiedades es faltante o no se cumple evitara que se siga con las siguientes lineas de codigo
                return BadRequest(ModelState);
            }

            //para validar el no meter 2 parametros con el mismo nombre. VillaDto es lo que estoy enviando
            if (await _db.Villas.FirstOrDefaultAsync(v => v.Nombre.ToLower() == createDto.Nombre.ToLower()) != null)
            {
                ModelState.AddModelError("NombreExiste", "La Villa con ese nombre ya existe perra");
                return BadRequest(ModelState);
            }

            if (createDto == null)
            {
                return BadRequest(createDto);
            }




            //villaDto.Id = VillaStore.villaList.OrderByDescending(v => v.Id).FirstOrDefault().Id + 1;
            //VillaStore.villaList.Add(villaDto); //agregamos un registro nuevo a la lista

            
            /*Antes de usar mapper
            Villa modelo = new()
            {
                Nombre = villaDto.Nombre,
                Detalle = villaDto.Detalle,
                ImagenUrl = villaDto.ImagenUrl,
                Ocupantes = villaDto.Ocupantes,
                Tarifa = villaDto.Tarifa,
                MetrosCuadrados = villaDto.MetrosCuadrados,
                Amenidad = villaDto.Amenidad
            };
            */

            //esta linea de codigo reemplaza todo el modelo de arriba
            Villa modelo = _mapper.Map<Villa>(createDto);

            //insert
            await _db.Villas.AddAsync(modelo);
            //save DB
            await _db.SaveChangesAsync();

            //cuando se crea un recurso se indica la url del recurso creado, en este caso el HTTPGET. 
            //el endpoint requiere un id
            return CreatedAtRoute("GetVilla", new {id = modelo.Id}, modelo);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task <IActionResult> DeleteVilla(int id)
        {
            if(id==0)
            {
                return BadRequest();
            }

            //la variable id es lo que yo tengo en la lista
            //Si uno de los ID que estamos enviando empata con los que tenemos en la lista
            var villa = await _db.Villas.FirstOrDefaultAsync(v => v.Id == id);
            if(villa == null)
            {
                //esto pasara si no empata ningun villa con tal ID, entonces no borra nada por no existir
                return NotFound();
            }

            //Nos vamos a nuestra lista para eliminar
            //VillaStore.villaList.Remove(villa);

            //medio db context
            _db.Villas.Remove(villa);
            //se registren cambios
            await _db.SaveChangesAsync();

            //cuando es delete, es no content, de no contenido ya que se borro.
            return NoContent();
        }


        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task <IActionResult>UpdateVilla(int id, [FromBody] VillaUpdateDto updateDto)
        {
            if(updateDto == null || id != updateDto.Id)
            {
                return BadRequest(); 
            }

            //tengo el registro antes de actualizarlo
            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);
            //villa.Nombre = villaDto.Nombre;
            //villa.Ocupantes = villaDto.Ocupantes;
            //villa.MetrosCuadrados = villaDto.MetrosCuadrados;


            /*
            Villa modelo = new()
            {
                Id = villaDto.Id,
                Nombre = villaDto.Nombre,
                Detalle = villaDto.Detalle, 
                ImagenUrl = villaDto.ImagenUrl, 
                Ocupantes = villaDto.Ocupantes, 
                Tarifa = villaDto.Tarifa,   
                MetrosCuadrados = villaDto.MetrosCuadrados,
                Amenidad = villaDto.Amenidad
            };
            */

            //nuevo modelo
            Villa modelo = _mapper.Map<Villa>(updateDto);

            _db.Villas.Update(modelo);
            await _db.SaveChangesAsync();
            return NoContent();

        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task <IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> patchDto)
        {
            if (patchDto == null || id == 0)
            {
                return BadRequest();
            }

            //tengo el registro antes de actualizarlo
            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id == id);

            var villa = await _db.Villas.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);

            /*
            VillaUpdateDto villaDto = new()
            {
                Id = villa.Id,
                Nombre = villa.Nombre,
                Detalle = villa.Detalle,
                ImagenUrl = villa.ImagenUrl,
                Ocupantes = villa.Ocupantes,
                Tarifa = villa.Tarifa,
                MetrosCuadrados = villa.MetrosCuadrados,
                Amenidad = villa.Amenidad
            };
            */

            //NUEVO METODO
            VillaUpdateDto villaDto = _mapper.Map<VillaUpdateDto>(villa);

            if (villa == null) return BadRequest();

            //Metodo Applyto, le enviamos registro actual y capturamos model state. //Json Patch por eso se descargaron los nugets
            patchDto.ApplyTo(villaDto, ModelState);

            //validar si no es valido
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            /*
            Villa modelo = new()
            {
                Id = villaDto.Id,
                Nombre =  villaDto.Nombre,
                Detalle = villaDto.Detalle,
                ImagenUrl = villaDto.ImagenUrl,
                Ocupantes = villaDto.Ocupantes,
                Tarifa = villaDto.Tarifa,
                MetrosCuadrados = villaDto.MetrosCuadrados,
                Amenidad = villaDto.Amenidad
            };
            */

            Villa modelo = _mapper.Map<Villa>(villaDto);

            _db.Villas.Update(modelo);
            await _db.SaveChangesAsync();
            return NoContent();

        }
    }
}
