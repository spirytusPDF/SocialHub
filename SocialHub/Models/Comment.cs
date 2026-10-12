namespace SocialHub.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Author { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        public int postId { get; set; }
        public Post? Post { get; set; }
    }
}
