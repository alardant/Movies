using System.ComponentModel.DataAnnotations;

namespace Movies.DTO
{
    public class MovieDto
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Author { get; set; }

        [Required]
        public MovieGenre Genre { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfRelease { get; set; }
    }

}
