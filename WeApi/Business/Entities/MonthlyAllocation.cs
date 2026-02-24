using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Business.Entities;

[Table("MonthlyAllocations")]
public class MonthlyAllocation
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [MaxLength(7)]
    public string Month { get; set; } = string.Empty;

    public int PointsRemaining { get; set; } = 10;

    public int PointsReceived { get; set; } = 0;

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
}
