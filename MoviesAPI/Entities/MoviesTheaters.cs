namespace MoviesAPI.Entities
{
    public class MoviesTheaters
    {
        public int TheaterId { get; set; }
        public int MovieId { get; set; }
        public Theater Theater { get; set; }
        public Movie Movie { get; set; }
    }
}
