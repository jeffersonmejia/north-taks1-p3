namespace NorthwindStore.Models.Common;

public abstract class SoftDeleteEntity
{
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
}
