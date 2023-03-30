namespace MoviesAPI.DTOs
{
    public class MoviePutGetDTO
    {
        public MovieDTO Movie { get; set; }
        public List<GenreDTO> SelectedGenres { get; set; }
        public List<GenreDTO> NonSelectedGenres { get; set; } 
        public List<TheaterDTO> SelectedTheaters { get; set; }
        public List<TheaterDTO> NonSelectedTheaters { get; set; }
        public List<ActorsMoviesDTO> Actors { get; set; }
    }
}
