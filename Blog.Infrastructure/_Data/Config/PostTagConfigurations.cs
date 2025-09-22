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
    public class PostTagConfigurations : IEntityTypeConfiguration<PostTag>
    {
        public void Configure(EntityTypeBuilder<PostTag> builder)
        {
            builder.HasKey(PT => new { PT.PostId, PT.TagId });
            builder.HasOne(PT => PT.Tag).WithMany(T=>T.postTags).HasForeignKey(PT=>PT.TagId);
            builder.HasOne(PT => PT.Post).WithMany(T=>T.postTags).HasForeignKey(PT=>PT.PostId);
        }
    }
}
