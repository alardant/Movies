using System.ComponentModel.DataAnnotations;

namespace MovieMaker.DTO
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
        public string Genre { get; set; }
        [Required]
        public string DateOfRelease { get; set; }
    }

}
