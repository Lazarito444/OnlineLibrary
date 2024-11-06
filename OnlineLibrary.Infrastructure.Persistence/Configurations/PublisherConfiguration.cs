using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineLibrary.Core.Domain.Entities;

namespace OnlineLibrary.Infrastructure.Persistence.Configurations;

public class PublisherConfiguration : IEntityTypeConfiguration<Publisher>
{
    public void Configure(EntityTypeBuilder<Publisher> builder)
    {
        builder.ToTable("Publishers");
        builder.HasKey(publisher => publisher.Id);

        builder.HasMany(publisher => publisher.Books)
            .WithOne(book => book.Publisher)
            .HasForeignKey(book => book.PublisherId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}