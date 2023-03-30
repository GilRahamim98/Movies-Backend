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

        [HttpGet] // api/movies
        public async Task<ActionResult<HomePageDTO>> Get()
        {
            var top = 6;
            var today = DateTime.Today;
            var allMovies=await context.Movies.OrderBy(x=>x.ReleaseDate).Take(top).ToListAsync();
            var inTheaters = await context.Movies.Where(x => x.InTheaters).OrderBy(x => x.ReleaseDate).Take(top).ToListAsync();

            var homePageDTO= new HomePageDTO();
            homePageDTO.MoviesList = mapper.Map<List<MovieDTO>>(allMovies);
            homePageDTO.InTheaters=mapper.Map<List<MovieDTO>>(inTheaters);
            return homePageDTO;

           

        }
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

        [HttpGet("filter")]
        public async Task<ActionResult<List<MovieDTO>>> Filter([FromQuery] FilterMoviesDTO filterMoviesDTO)
        {
           
            var moviesQueryable = context.Movies.AsQueryable();
            if (!string.IsNullOrEmpty(filterMoviesDTO.Title))
            {
                moviesQueryable=moviesQueryable.Where(x=>x.Title.Contains(filterMoviesDTO.Title));
            }
            if (filterMoviesDTO.InTheaters)
            {
                moviesQueryable = moviesQueryable.Where(x => x.InTheaters);
            }
          
            if (filterMoviesDTO.GenreID!=0)
            {
                moviesQueryable = moviesQueryable.Where(x => x.MoviesGenres.Select(y=>y.GenreId).Contains(filterMoviesDTO.GenreID));
            }
            await HttpContext.InsertParametersPagnationInHeader(moviesQueryable);
            var movies = await moviesQueryable.OrderBy(x => x.Title).Paginate(filterMoviesDTO.PaginationDTO).ToListAsync();
            

            return mapper.Map<List<MovieDTO>>(movies);


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

        
        [HttpGet("putget/{id:int}")]
        public async Task<ActionResult<MoviePutGetDTO>> PutGet(int id)
        {
            var movieActionResult = await Get(id);
            if(movieActionResult.Result is NotFoundResult) { return NotFound(); }
            var movie = movieActionResult.Value;
            var genresSelectedIds = movie.Genres.Select(x=>x.id).ToList();
            var nonSelectedGenres = await context.Genres.Where(x => !genresSelectedIds.Contains(x.id)).ToListAsync();

            var theatersSelectedIds = movie.Theaters.Select(x => x.id).ToList();
            var nonSelectedTheaters = await context.Theaters.Where(x => !theatersSelectedIds.Contains(x.id)).ToListAsync();

            var nonSelectedGenresDTOs = mapper.Map<List<GenreDTO>>(nonSelectedGenres);
            var nonSelectedTheatersDTOs = mapper.Map<List<TheaterDTO>>(nonSelectedTheaters);
            var response = new MoviePutGetDTO();
            response.Movie = movie;
            response.SelectedGenres = movie.Genres;
            response.NonSelectedGenres=nonSelectedGenresDTOs;
            response.SelectedTheaters = movie.Theaters;
            response.NonSelectedTheaters=nonSelectedTheatersDTOs;
            response.Actors = movie.Actors;
            return response;
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromForm] MovieCreationDTO movieCreationDTO)
        {
            var movie = await context.Movies.Include(x => x.MoviesActors).Include(x => x.MoviesGenres).Include(x => x.MoviesTheaters).FirstOrDefaultAsync(x => x.id == id);
            if (movie == null)
            {
               return NotFound();
            }
             movie = mapper.Map(movieCreationDTO, movie);



            if (movieCreationDTO.Poster != null) 
            {
                movie.Poster=await fileStorageService.EditFile(container,movieCreationDTO.Poster,movie.Poster);
            }



            AnnotateActorsOrder(movie);
            
            await context.SaveChangesAsync();
            
            return NoContent();
            
        }
        
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var movie = await context.Movies.FirstOrDefaultAsync(x => x.id == id);
            if (movie==null)
            {
                return NotFound();
            }
            context.Remove(movie);
            await context.SaveChangesAsync();
            await fileStorageService.DeleteFile(movie.Poster, container);
            return NoContent();

        }

    }
}
