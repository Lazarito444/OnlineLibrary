namespace OnlineLibrary.Core.Domain.Entities;

public class Author
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public ICollection<Book> Books { get; set; }
}