namespace SFA.DAS.LearnerData.Data.Entities;

public enum EntityStatus
{
    New,
    Existing
}

public interface IModificationHistory
{
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}


public abstract class Entity : IModificationHistory
{
    public long Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public EntityStatus EntityStatus => Id <= 0 ? EntityStatus.New : EntityStatus.Existing;
}