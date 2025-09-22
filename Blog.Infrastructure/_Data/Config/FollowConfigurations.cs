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
    public class FollowConfigurations : IEntityTypeConfiguration<Follow>
    {
        public void Configure(EntityTypeBuilder<Follow> builder)
        {
            builder.HasKey(F => new {F.FollowerId,F.FollowedId});
            builder.HasOne(F=>F.Follower).WithMany(U=>U.Following).HasForeignKey(F=>F.FollowerId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(F=>F.Followed).WithMany(U=>U.Followers).HasForeignKey(F=>F.FollowedId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
