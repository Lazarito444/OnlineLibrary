using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineLibrary.Core.Domain.Entities;
using OnlineLibrary.Core.Domain.Enums;

namespace OnlineLibrary.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(user => user.Id);

        builder.HasData(new User
            {
                Id = 1,
                DateOfBirth = new DateTime(2005, 10, 13),
                Email = "ariellazaro444@gmail.com",
                FullName = "Ariel David Lázaro Pérez",
                Password = "ariel123",
                Role = Roles.Admin
            }
        );
    }
}