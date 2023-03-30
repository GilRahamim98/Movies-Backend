using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using MoviesAPI.Helpers;
using System.Linq;

namespace MoviesAPI.Controllers
{
    [Route("api/genres")]
    [ApiController]
    public class GenresController:ControllerBase
    {
        private readonly ILogger<GenresController> logger;
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;

        public GenresController(ILogger<GenresController> logger
            ,ApplicationDBContext context,
            IMapper mapper)
        {
            this.logger = logger;
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet] // api/genres
        public async Task<ActionResult<List<GenreDTO>>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            var queryable=context.Genres.AsQueryable();
            await HttpContext.InsertParametersPagnationInHeader(queryable);
            var genres= await queryable.OrderBy(x=>x.Name).Paginate(paginationDTO).ToListAsync();
            return mapper.Map<List<GenreDTO>>(genres);
          
        }

        [HttpGet("all")] // api/genres/all
        public async Task<ActionResult<List<GenreDTO>>> Get()
        {
            var genres = await context.Genres.OrderBy(x => x.Name).ToListAsync();
            return mapper.Map<List<GenreDTO>>(genres);

        }
        [HttpGet("{id:int}")]
        public async Task<ActionResult<GenreDTO>> Get(int id)
        {
            var genre = await context.Genres.FirstOrDefaultAsync(x => x.id==id);
            if (genre == null)
            {
                return NotFound();
            }
            return mapper.Map<GenreDTO>(genre);



        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] GenreCreationDTO genreCreationDTO)
        {
            var genre=mapper.Map<Genre>(genreCreationDTO);
            context.Add(genre);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id,[FromBody] GenreCreationDTO genreCreationDTO)
        {
            var genre = await context.Genres.FirstOrDefaultAsync(x => x.id == id);
            if (genre == null)
            {
                return NotFound();
            }
            genre = mapper.Map(genreCreationDTO, genre);
            await context.SaveChangesAsync();
            return NoContent();

        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var exists = await context.Genres.AnyAsync(x => x.id == id);
            if (!exists)
            {
                return NotFound();
            }
            context.Remove(new Genre() { id = id });
            await context.SaveChangesAsync();
            return NoContent();

        }



    }
}
