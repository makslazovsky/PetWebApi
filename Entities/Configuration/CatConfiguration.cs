using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Configuration
{
    public class CatConfiguration : IEntityTypeConfiguration<Cat>
    {
        public void Configure(EntityTypeBuilder<Cat> builder)
        {
            builder.HasData
                (
                new Cat
                {
                    Id = new Guid("12abbca8-664d-4b20-b5de-024705497d4a"),
                    Name = "Barsik",
                    EmployeeId = new Guid("80abbca8-664d-4b20-b5de-024705497d4a")
                }
                );
                
        }

        }
}

