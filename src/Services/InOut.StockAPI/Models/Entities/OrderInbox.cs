using System.ComponentModel.DataAnnotations;

namespace InOut.StockAPI.Models.Entities;

public class OrderInbox
{
    [Key]
    public Guid IdempotentToken { get; set; }
    public bool Processed { get; set; }
    public string Payload { get; set; }
}
