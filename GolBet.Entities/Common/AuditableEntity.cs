// GolBet.Entities/Common/AuditableEntity.cs
namespace GolBet.Entities.Common;

public abstract class AuditableEntity
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public bool IsActive { get; set; } = true;
}