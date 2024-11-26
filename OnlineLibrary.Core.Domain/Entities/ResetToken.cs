namespace OnlineLibrary.Core.Domain.Entities;

public class ResetToken
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public Guid Token { get; set; } = Guid.NewGuid();
    public User User { get; set; }
}