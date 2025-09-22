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
    internal class CommentConfigurations : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.HasOne(C => C.Post).WithMany(P=>P.Comments).HasForeignKey(C=>C.PostId);
            builder.HasOne(C => C.ApplicationUser).WithMany(U=>U.Comments).HasForeignKey(C=>C.ApplicationUserId).OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
