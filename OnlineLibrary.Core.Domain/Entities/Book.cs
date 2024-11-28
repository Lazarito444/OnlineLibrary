using OnlineLibrary.Core.Domain.Enums;

namespace OnlineLibrary.Core.Domain.Entities;

public class Book
{
    public int Id { get; set; }
    public int PublisherId { get; set; }
    public int AuthorId { get; set; }
    public int PagesCount { get; set; }
    public string Title { get; set; }
    public string Synopsis { get; set; }
    public string ImageUrl { get; set; }
    public DateTime PublishedDate { get; set; }
    public BookGenre BookGenre { get; set; }
    public Publisher Publisher { get; set; }
    public Author Author { get; set; }
}