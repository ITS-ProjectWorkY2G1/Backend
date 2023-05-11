using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.WatchModels
{
    [Table("smartwatches", Schema = "pw_gruppo1")]
    public class Smartwatch
    {
        [Column("id")]
        public Guid Id { get; set; }
        [Column("position")]
        public string Position { get; set; } = string.Empty;
        [Column("timestamp")]
        public DateTime Timestamp { get; set; }
        [Column("heart_rate")]
        public int HeartRate { get; set; }
        [Column("session_id")]
        public Guid SessionId { get; set; }
        [Column("user_id")]
        public Guid UserId { get; set; }
    }
}
