using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Entities
{
    public class Genre
    {
        public int id { get; set; }
        [Required(ErrorMessage ="זהו שדה חובה!")]
        [StringLength(50)]
        public string Name { get; set; }
    }
}
