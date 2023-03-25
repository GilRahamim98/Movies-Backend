using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using MoviesAPI.Helpers;

namespace MoviesAPI.Controllers
{
    [Route("api/movies")]
    [ApiController]
    public class MoviesController:ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;
        private readonly IFileStorageService fileStorageService;
        private string container = "movies";

        public MoviesController(ApplicationDBContext context,
            IMapper mapper,IFileStorageService fileStorageService)
        {
            this.context = context;
            this.mapper = mapper;
            this.fileStorageService = fileStorageService;
        }

       /* [HttpGet] // api/movies
        public async Task<ActionResult<List<GenreDTO>>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            var queryable = context.Genres.AsQueryable();
            await HttpContext.InsertParametersPagnationInHeader(queryable);
            var genres = await queryable.OrderBy(x => x.Name).Paginate(paginationDTO).ToListAsync();
            return mapper.Map<List<GenreDTO>>(genres);

        }*/
        [HttpGet("{id:int}")]
        public async Task<ActionResult<MovieDTO>> Get(int id)
        {
            var movie = await context.Movies.Include(x => x.MoviesGenres)
                .ThenInclude(x=>x.Genre).Include(x=>x.MoviesTheaters).ThenInclude(x=>x.Theater)
                .Include(x => x.MoviesActors).ThenInclude(x => x.Actor).FirstOrDefaultAsync(x=>x.id==id);
            if (movie == null)
            {
                return NotFound();
            }
            var dto = mapper.Map<MovieDTO>(movie);
            dto.Actors=dto.Actors.OrderBy(x=>x.Order).ToList();
            return dto;
        }
        [HttpGet("PostGet")]
        public async Task<ActionResult<MoviePostGetDTO>> PostGet()
        {
            var theaters =await context.Theaters.ToListAsync();
            var genres = await context.Genres.ToListAsync();
            var theatersDTO = mapper.Map<List<TheaterDTO>>(theaters);
            var genresDTO=mapper.Map<List<GenreDTO>>(genres);
            return new MoviePostGetDTO() { Genres=genresDTO,Theaters=theatersDTO };
        }

        [HttpPost]
        public async Task<ActionResult<int>> Post([FromForm] MovieCreationDTO movieCreationDTO)
        {
            var movie = mapper.Map<Movie>(movieCreationDTO);
            if(movieCreationDTO.Poster != null)
            {
                movie.Poster = await fileStorageService.SaveFile(container, movieCreationDTO.Poster);
            }
            AnnotateActorsOrder(movie);
            context.Add(movie);
            await context.SaveChangesAsync();
            return movie.id;
        }

        private void AnnotateActorsOrder(Movie movie)
        {
            if(movie.MoviesActors != null)
            {
                for(int i = 0; i < movie.MoviesActors.Count; i++)
                {
                    movie.MoviesActors[i].Order = i;
                }
            }
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] GenreCreationDTO genreCreationDTO)
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
