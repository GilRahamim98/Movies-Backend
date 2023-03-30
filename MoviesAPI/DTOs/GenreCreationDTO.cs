using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.DTOs
{
    public class GenreCreationDTO
    {
        [Required(ErrorMessage = "זהו שדה חובה!")]
        [StringLength(50)]
        public string Name { get; set; }
    }
}
