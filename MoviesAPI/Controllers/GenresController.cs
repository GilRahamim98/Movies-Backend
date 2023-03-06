using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Services;
using MoviesAPI.Entities;

namespace MoviesAPI.Controllers
{
    [Route("api/genres")]
    [ApiController]
    public class GenresController:ControllerBase
    {
        private readonly IRepository repository;

        public GenresController(IRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<List<Genre>>> Get()
        {
            return await repository.GetAllGenres();
        }
        [HttpGet("{id:int}")]
        public ActionResult<Genre> Get(int id,string param2)
        {
            
            var genre = repository.GetGenreById(id);
            if (genre == null)
            {
               return NotFound();
            }
            return genre;
        }

        [HttpPost]
        public ActionResult Post([FromBody] Genre genre)
        {
            repository.AddGenre(genre);
    
            return NoContent();

        }

        [HttpPut]
        public ActionResult Put()
        {
            return NoContent();


        }
        [HttpDelete]
        public ActionResult Delete()
        {
            return NoContent();


        }



    }
}
