namespace SocialHub.Models
{
    public class Post
    {
        public int Id { get; set; }
        public string Author { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public List<Comment>? comments { get; set; }

        public int? userId {  get; set; }
        public User? user { get; set; }
    }
}
