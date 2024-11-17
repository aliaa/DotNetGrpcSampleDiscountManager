using System.ComponentModel.DataAnnotations;

namespace DiscountManager.Server.Entities;


public class DiscountCode
{
    [Key]
    [StringLength(8)]
    public string Code { get; set; }

    public CodeStatus Status { get; set; }

    [Timestamp]
    public byte[] Version { get; set; }
}

public enum CodeStatus
{
    ReadyToUse,
    Assigned,
    Used
}
