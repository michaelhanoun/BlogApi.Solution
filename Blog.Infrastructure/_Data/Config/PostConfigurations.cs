using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.Core.Entities;
using Blog.Core.Entities.Post_Aggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blog.Infrastructure._Data.Config
{
    internal class PostConfigurations : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.HasOne(P => P.ApplicationUser).WithMany(U=>U.Posts).HasForeignKey(P => P.ApplicationUserId);
            builder.HasMany(P => P.PostCategories).WithOne(PC => PC.Post).HasForeignKey(PC=>PC.PostId);
            builder.Property(P => P.Status).HasConversion(o => o.ToString(), o => (Status)Enum.Parse(typeof(Status),o));
            builder.HasIndex(P => P.Slug);
        }
    }
}
