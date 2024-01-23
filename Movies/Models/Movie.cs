namespace MovieMaker.Models
{
    public class Movie
    {
        private int _id { get; set; }
        private string _title { get; set; }
        private string _description { get; set; }
        private string _author { get; set; }
        private MovieGenre _genre { get; set; }
        private DateTime dateOfRelease { get; set; }

        public int Id { get { return _id; } set { _id = value; } }
        public string Title { get { return _title; } set { _title = value; } }
        public string Description { get { return _description; } set { _description = value; } }
        public string Author { get { return _author; } set { _author = value; } }
        public MovieGenre Genre { get { return _genre; } set { _genre = value; } }
        public DateTime DateOfRelease { get { return dateOfRelease; } set { dateOfRelease = value; } }
    }
}