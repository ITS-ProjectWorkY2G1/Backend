using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.WatchModels
{
    [Table("sessions", Schema = "pw_gruppo1")]
    [Index(nameof(SessionId), IsUnique = true)]
    public class Session
    {
        [Key]
        [Column("session_id")]
        public Guid SessionId { get; set; }
        [Column("session_time")]
        public TimeSpan SessionTime { get; set; }
        [Column("avg_heart_rate")]
        public int AvgHeartRate { get; set; }
        [Column("session_distance")]
        public int SessionDistance { get; set; }
        [Column("pool_laps")]
        public short PoolLaps { get; set; }
        [Column("pool_length")]
        public short PoolLength { get; set; }
    }
}
