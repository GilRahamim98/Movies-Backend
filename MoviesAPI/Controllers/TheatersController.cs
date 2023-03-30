using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using MoviesAPI.Helpers;

namespace MoviesAPI.Controllers
{
    
    [Route("api/theaters")]
    [ApiController]
    public class TheatersController : ControllerBase
    {
        private readonly ApplicationDBContext context;
        private readonly IMapper mapper;
        public TheatersController(ApplicationDBContext context, IMapper mapper) 
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet] // api/theaters
        public async Task<ActionResult<List<TheaterDTO>>> Get([FromQuery] PaginationDTO paginationDTO)
        {
            var queryable = context.Theaters.AsQueryable();
            await HttpContext.InsertParametersPagnationInHeader(queryable);
            var theaters = await queryable.OrderBy(x => x.Name).Paginate(paginationDTO).ToListAsync();

            return mapper.Map<List<TheaterDTO>>(theaters);

        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<TheaterDTO>> Get(int id)
        {
            var theater = await context.Theaters.FirstOrDefaultAsync(x => x.id == id);
            if (theater == null)
            {
                return NotFound();
            }
            return mapper.Map<TheaterDTO>(theater);
        }

        [HttpPost]
        public async Task<ActionResult> Post( TheaterCreationDTO theaterCreationDTO)
        {
            var theater = mapper.Map<Theater>(theaterCreationDTO);
            Console.WriteLine(theater);
      
            context.Add(theater);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id,TheaterCreationDTO theaterCreationDTO)
        {
            var theater = await context.Theaters.FirstOrDefaultAsync(x => x.id == id);
            if (theater == null)
            {
                return NotFound();
            }
            theater = mapper.Map(theaterCreationDTO, theater);
         
            await context.SaveChangesAsync();
            return NoContent();

        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var theater = await context.Theaters.FirstOrDefaultAsync(x => x.id == id);
            if (theater == null)
            {
                return NotFound();
            }
            context.Remove(theater);
            await context.SaveChangesAsync();
            return NoContent();

        }

    }
}
