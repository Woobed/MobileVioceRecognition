using SQLite;

namespace VoiceRecognition.DBContext.Models
{
    [Table("Requests")]
    public class RequestModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(1000)]
        public string Text { get; set; } = string.Empty;
        [MaxLength(1000)]
        public string Url { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsFavorite { get; set; }
    }
}
