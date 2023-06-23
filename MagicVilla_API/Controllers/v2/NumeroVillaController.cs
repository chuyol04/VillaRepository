using AutoMapper;
using MagicVilla_API.Datos;
using MagicVilla_API.Modelos;
using MagicVilla_API.Modelos.Dto;
using MagicVilla_API.Repositorio.IRepositorio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace MagicVilla_API.Controllers.v2
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("2.0")]
    public class NumeroVillaController : ControllerBase
    {
        //Crear variable privada. Variables privadas con guiones bajo
        private readonly ILogger<NumeroVillaController> _logger;

        //interfaz villa repositorio
        private readonly IVillaRepositorio _villaRepo;
        private readonly INumeroVillaRepositorio _numeroRepo; //inyectar a numero villa controler

        private readonly IMapper _mapper;

        //clase api response en modelo, no se inyecta solo de inicializa
        protected APIResponse _response;

        //constructor donde pondremos servicio logger //inyectamos dependencias para poder inyectarlo y usarlo
        public NumeroVillaController(ILogger<NumeroVillaController> logger, IVillaRepositorio villaRepo,
                                                                            INumeroVillaRepositorio numeroRepo, IMapper mapper)
        {
            //se registra logger
            _logger = logger;
            _villaRepo = villaRepo;
            _numeroRepo = numeroRepo; //ya esta inyectado y se puede usar
            _mapper = mapper;
            _response = new();
        }

        /*
        //Este metodo va para la version 1.0
        [MapToApiVersion("1.0")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetNumeroVillas()
        {
            try
            {

                _logger.LogInformation("Obtener numeros de villas");

                IEnumerable<NumeroVilla> numeroVillaList = await _numeroRepo.ObtenerTodos(incluirPropiedades: "Villa");

                _response.Resultado = _mapper.Map<IEnumerable<NumeroVillaDto>>(numeroVillaList);
                _response.statusCode = HttpStatusCode.OK;


                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return _response;
        }
        */

        //solo endpoints a  la version 2
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "valor1", "valor2" };
        }

        /*
        //2- Obtenemos la informacion del Store pero solo un valor determinado, no una lista como en el 1.
        //Hay que diferenciar los endpionts ya que no se pueden tener 2 mismos del HTTPGET, se diferencia poniendo en su atributo
        //que en este caso será el ID
        [HttpGet("{id:int}", Name = "GetNumeroVilla")] //Le asignamos nombre
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]//codigos de estado
        [ProducesResponseType(StatusCodes.Status400BadRequest)]//codigos de estado
        [ProducesResponseType(StatusCodes.Status404NotFound)]//codigos de estado
        public async Task<ActionResult<APIResponse>> GetNumeroVilla(int id)
        {

            try
            {

                if (id == 0)
                {
                    _logger.LogError("Error al traer numero de villa con Id " + id);
                    _response.statusCode = HttpStatusCode.BadRequest;
                    _response.IsExitoso = false;
                    //Mala solicitud, codigo de estado 400
                    return BadRequest(_response);
                }


                //metodo asincrono, solo nos trae un registro el obtener
                var numerovilla = await _numeroRepo.Obtener(v => v.VillaNo == id, incluirPropiedades: "Villa");

                //villa sea nullo
                if (numerovilla == null)
                {
                    _response.statusCode = HttpStatusCode.NotFound;
                    _response.IsExitoso = false;
                    //no encontro ningun registro error 404
                    return NotFound(_response);
                }

                _response.Resultado = _mapper.Map<NumeroVillaDto>(numerovilla);
                _response.statusCode = HttpStatusCode.OK;

                //return Ok(villa); ANTES

                //Este solo retorna un solo objeto en base al ID, codigo 200 OK
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return _response;
        }
        */

        /*

        [HttpPost]//Crear
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] //Tamaño maximo del parametro
        public async Task<ActionResult<APIResponse>> CrearNumeroVilla([FromBody] NumeroVillaCreateDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    //si alguna de las propiedades es faltante o no se cumple evitara que se siga con las siguientes lineas de codigo
                    return BadRequest(ModelState);
                }

                //Verificar que no exista mismo numero de villa. Se compara villa de createdto si es diferente a nullo es porque ya existe
                if (await _numeroRepo.Obtener(v => v.VillaNo == createDto.VillaNo) != null)
                {
                    ModelState.AddModelError("ErrorMessages", "El numero de Villa ya existe");
                    return BadRequest(ModelState);
                }

                //no exista id padre. si es iogual a nullo no existe
                if (await _villaRepo.Obtener(v => v.Id == createDto.VillaId) == null)
                {
                    ModelState.AddModelError("ErrorMessages", "El Id de la villa no existe!!");
                    return BadRequest(ModelState);
                }

                if (createDto == null)
                {
                    return BadRequest(createDto);
                }



                //esta linea de codigo reemplaza todo el modelo de arriba
                NumeroVilla modelo = _mapper.Map<NumeroVilla>(createDto);

                //fecha del sistema hay que obtener info del modelo
                modelo.FechaCreacion = DateTime.Now;
                modelo.FechaActualizacion = DateTime.Now;

                //insert
                await _numeroRepo.Crear(modelo);

                _response.Resultado = modelo;
                _response.statusCode = HttpStatusCode.Created;

                //cuando se crea un recurso se indica la url del recurso creado, en este caso el HTTPGET. 
                //el endpoint requiere un id
                return CreatedAtRoute("GetNumeroVilla", new { id = modelo.VillaNo }, _response);
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return _response;
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteNumeroVilla(int id)
        {
            try
            {

                if (id == 0)
                {
                    _response.IsExitoso = false;
                    _response.statusCode = HttpStatusCode.BadRequest;
                    //esto pasara si no empata ningun villa con tal ID, entonces no borra nada por no existir
                    return NotFound(_response);
                }

                var numerovilla = await _numeroRepo.Obtener(v => v.VillaNo == id);

                //me lo revuelve
                if (numerovilla == null)
                {
                    _response.IsExitoso = false;
                    _response.statusCode = HttpStatusCode.NotFound;
                    return NotFound(_response);
                }

                //medio db context, lo elimina
                await _numeroRepo.Remover(numerovilla);

                _response.statusCode = HttpStatusCode.NoContent;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsExitoso = false;
                _response.ErrorMessages = new List<string>() { ex.ToString() };
            }

            return BadRequest(_response);

        }


        [HttpPut("{id:int}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateNumeroVilla(int id, [FromBody] NumeroVillaUpdateDto updateDto)
        {
            if (updateDto == null || id != updateDto.VillaNo)
            {
                _response.IsExitoso = false;
                _response.statusCode = HttpStatusCode.BadRequest;
                return BadRequest(_response);
            }

            if (await _villaRepo.Obtener(V => V.Id == updateDto.VillaId) == null)
            {
                ModelState.AddModelError("ErrorMessages", "El id de la villa no existe");
                return BadRequest(ModelState);
            }


            //nuevo modelo
            NumeroVilla modelo = _mapper.Map<NumeroVilla>(updateDto);

            await _numeroRepo.Actualizar(modelo);
            _response.statusCode = HttpStatusCode.NoContent;
            return Ok(_response);

        }

        */


    }
}
