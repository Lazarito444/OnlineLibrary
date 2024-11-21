using OnlineLibrary.Core.Domain.Enums;

namespace OnlineLibrary.Core.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public Roles Role { get; set; }
}