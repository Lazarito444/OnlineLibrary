using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineLibrary.Core.Domain.Entities;

namespace OnlineLibrary.Infrastructure.Persistence.Configurations;

public class BorrowedBookConfiguration : IEntityTypeConfiguration<BorrowedBook>
{
    public void Configure(EntityTypeBuilder<BorrowedBook> builder)
    {
        builder.ToTable("BorrowedBooks");
        builder.HasKey(bb => bb.Id);

        builder.HasOne<Book>()
            .WithOne()
            .HasForeignKey<BorrowedBook>(bb => bb.BookId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<User>()
            .WithOne()
            .HasForeignKey<BorrowedBook>(bb => bb.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}