using Microsoft.AspNetCore.Mvc;

using MoviesAPI.Entities;


namespace MoviesAPI.Controllers
{
    [Route("api/genres")]
    [ApiController]
    public class GenresController:ControllerBase
    {
        private readonly ILogger<GenresController> logger;

        public GenresController(ILogger<GenresController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<Genre>>> Get()
        {
            return new List<Genre>() { new Genre() {id=1,Name="דרמה" } };
        }
        [HttpGet("{id:int}")]
        public ActionResult<Genre> Get(int id)
        {   
            throw new NotImplementedException();
            
            
        }

        [HttpPost]
        public ActionResult Post([FromBody] Genre genre)
        {
            throw new NotImplementedException();
        }

        [HttpPut]
        public ActionResult Put([FromBody] Genre genre)
        {
            throw new NotImplementedException();


        }
        [HttpDelete]
        public ActionResult Delete()
        {
            throw new NotImplementedException();


        }



    }
}
