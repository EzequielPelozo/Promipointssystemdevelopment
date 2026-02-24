using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Business.Entities;

[Table("PointAssignments")]
public class PointAssignment
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int FromUserId { get; set; }

    [Required]
    public int ToUserId { get; set; }

    [Required]
    public int Points { get; set; }

    [Required]
    [MaxLength(100)]
    public string Category { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Message { get; set; }

    public long Timestamp { get; set; }

    [Required]
    [MaxLength(7)]
    public string Month { get; set; } = string.Empty;

    [ForeignKey(nameof(FromUserId))]
    public User FromUser { get; set; } = null!;

    [ForeignKey(nameof(ToUserId))]
    public User ToUser { get; set; } = null!;
}
