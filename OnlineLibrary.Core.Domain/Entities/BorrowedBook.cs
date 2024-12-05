namespace OnlineLibrary.Core.Domain.Entities;

public class BorrowedBook
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public int UserId { get; set; }
    public bool Returned { get; set; }
    public DateTime BorrowDate { get; set; }
    public DateTime MaxReturnDate => BorrowDate.AddDays(14);
    public Book Book { get; set; }
    public User User { get; set; }
}