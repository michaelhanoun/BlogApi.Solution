using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Blog.Infrastructure._Data.Config
{
    internal class LikeConfigurations : IEntityTypeConfiguration<Like>
    {
        public void Configure(EntityTypeBuilder<Like> builder)
        {
            builder.HasOne(L => L.Post).WithMany(P => P.Likes).HasForeignKey(L=>L.PostId);
            builder.HasOne(L => L.ApplicationUser).WithMany(U=>U.Likes).HasForeignKey(L=>L.ApplicationUserId).OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}
