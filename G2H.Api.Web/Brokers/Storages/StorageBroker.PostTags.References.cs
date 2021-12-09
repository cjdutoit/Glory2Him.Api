// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using G2H.Api.Web.Models.PostTags;
using Microsoft.EntityFrameworkCore;

namespace G2H.Api.Web.Brokers.Storages
{
    public partial class StorageBroker
    {
        private static void AddPostTagReferences(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PostTag>()
                .HasOne(postTag => postTag.Post)
                .WithMany(post => post.PostTags)
                .HasForeignKey(postTag => postTag.PostId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PostTag>()
                .HasOne(postTag => postTag.Tag)
                .WithMany(post => post.PostTags)
                .HasForeignKey(postTag => postTag.TagId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PostTag>()
                .HasOne(postTag => postTag.Approval)
                .WithMany(approval => approval.PostTags)
                .HasForeignKey(postTag => postTag.ApprovalId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
