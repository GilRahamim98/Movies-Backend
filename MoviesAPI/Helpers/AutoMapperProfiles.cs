using AutoMapper;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using NetTopologySuite.Geometries;

namespace MoviesAPI.Helpers
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles(GeometryFactory geometryFactory) 
        {
            CreateMap<GenreDTO, Genre>().ReverseMap();
            CreateMap<GenreCreationDTO, Genre>();
            CreateMap<ActorDTO, Actor>().ReverseMap();
            CreateMap<ActorCreationDTO, Actor>().ForMember(x=>x.Picture,options=>options.Ignore());
            CreateMap<Theater, TheaterDTO>()
                .ForMember(x => x.Latitude, dto => dto.MapFrom(prop => prop.Location.Y))
                .ForMember(x => x.Longitude, dto => dto.MapFrom(prop => prop.Location.X));
            CreateMap<TheaterCreationDTO, Theater>()
                .ForMember(x=>x.Location,x=>x.MapFrom(dto=>
                geometryFactory.CreatePoint(new Coordinate(dto.Longitude, dto.Latitude))));
            CreateMap<MovieCreationDTO,Movie>()
                .ForMember(x=>x.Poster,options=>options.Ignore())
                .ForMember(x=>x.MoviesGenres,options=>options.MapFrom(MapMoviesGenres))
                .ForMember(x => x.MoviesTheaters, options => options.MapFrom(MapMoviesTheaters))
                .ForMember(x => x.MoviesActors, options => options.MapFrom(MapMoviesActors));
            CreateMap<Movie, MovieDTO>()
                .ForMember(x => x.Genres, options => options.MapFrom(MapMoviesGenres))
                .ForMember(x => x.Theaters, options => options.MapFrom(MapMoviesTheaters))
                .ForMember(x => x.Actors, options => options.MapFrom(MapMoviesActors));
        }

        private List<ActorsMoviesDTO> MapMoviesActors(Movie movie, MovieDTO moviedto)
        {
            var result = new List<ActorsMoviesDTO>();
            if (movie.MoviesActors != null)
            {
                foreach (var moviesActor in movie.MoviesActors)
                {
                    result.Add(new ActorsMoviesDTO() { id = moviesActor.ActorId, Name = moviesActor.Actor.Name,Character=moviesActor.Character,Picture=moviesActor.Actor.Picture,Order=moviesActor.Order  });
                }
            }
            return result;
        }

        private List<TheaterDTO> MapMoviesTheaters(Movie movie, MovieDTO moviedto)
        {
            var result = new List<TheaterDTO>();
            if (movie.MoviesTheaters != null)
            {
                foreach (var theater in movie.MoviesTheaters)
                {
                    result.Add(new TheaterDTO() { id = theater.TheaterId, Name = theater.Theater.Name,Latitude=theater.Theater.Location.Y,Longitude=theater.Theater.Location.X });
                }
            }
            return result;
        }



            private List<GenreDTO> MapMoviesGenres(Movie movie,MovieDTO moviedto)
        {
            var result = new List<GenreDTO>();
            if (movie.MoviesGenres != null) 
            {
                foreach (var genre in movie.MoviesGenres)
                {
                    result.Add(new GenreDTO() { id = genre.GenreId,Name=genre.Genre.Name });
                }
            }
            
            return result;
        }

        private List<MoviesGenres> MapMoviesGenres(MovieCreationDTO movieCreationDTO,Movie movie)
        {
            var result = new List<MoviesGenres>();
            if (movieCreationDTO.GenresIds == null) { return result; }
            foreach(var id in movieCreationDTO.GenresIds)
            {
                result.Add(new MoviesGenres() { GenreId = id });
            }
            return result;
        }
        private List<MoviesTheaters> MapMoviesTheaters(MovieCreationDTO movieCreationDTO,Movie movie)
        {
            var result = new List<MoviesTheaters>();
            if (movieCreationDTO.TheatersIds == null) { return result; }
            foreach (var id in movieCreationDTO.TheatersIds)
            {
                result.Add(new MoviesTheaters() { TheaterId = id });
            }
            return result;
        }

        private List<MoviesActors> MapMoviesActors(MovieCreationDTO movieCreationDTO, Movie movie)
        {
            var result = new List<MoviesActors>();
            if (movieCreationDTO.Actors == null) { return result; }
            foreach (var actor in movieCreationDTO.Actors)
            {
                result.Add(new MoviesActors() { ActorId = actor.id ,Character=actor.Character});
            }
            return result;
        }

    }
}
