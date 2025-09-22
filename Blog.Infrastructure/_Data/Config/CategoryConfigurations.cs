using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blog.Infrastructure._Data.Config
{
    internal class CategoryConfigurations : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasMany(C => C.PostCategories).WithOne(PC => PC.Category).HasForeignKey(PC => PC.CategoryId);
            builder.HasIndex(C => C.Name).IsUnique();
            builder.HasIndex(C => C.Slug).IsUnique();
        }
    }
}
