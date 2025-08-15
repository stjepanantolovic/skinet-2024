using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config
{
    public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(
                new IdentityRole { Id = "7cd9d41c-54ad-459b-8fcd-af55d1933022", Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = "35ce2b29-16bd-4558-a270-71a911d9ff34", Name = "Custommer", NormalizedName = "CUSTOMER" },
                new IdentityRole { Id = "40409cd5-aeaf-4f4f-b82a-0247ed7d684b", Name = "Supplier", NormalizedName = "SUPPLIER" }
            );
        }
    }
}