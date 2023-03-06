using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Entities
{
    public class Genre
    {
        public int id { get; set; }
        [Required(ErrorMessage ="זהו שדה חובה!")]
        [StringLength(15)]
        public string Name { get; set; }
    }
}
