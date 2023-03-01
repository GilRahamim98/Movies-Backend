using MoviesAPI.Entities;
namespace MoviesAPI.Services
{
    public class InMemoryRepository : IRepository
    {
        private List<Genre> _genres;

        public InMemoryRepository()
        {
            _genres = new List<Genre>()
            {
                new Genre(){id=1,Name="דרמה"},
                new Genre(){id=2,Name="אקשן"}
            };
        }
        public List<Genre> GetAllGenres()
        {
            return _genres;
        }
    }
}
