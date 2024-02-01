using System.ComponentModel.DataAnnotations.Schema;

namespace MovieMaker.Models
{
    public class Movie
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public MovieGenre Genre { get; set; }
        public DateTime DateOfRelease { get; set; }
        [ForeignKey("User")]
        public string? UserId { get; set; }
        public User User { get; set; }

    }
}