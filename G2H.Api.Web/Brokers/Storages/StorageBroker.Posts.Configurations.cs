// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using G2H.Api.Web.Models.Posts;
using Microsoft.EntityFrameworkCore;

namespace G2H.Api.Web.Brokers.Storages
{
    public partial class StorageBroker
    {
        private static void AddPostReferences(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Post>()
                .ToTable(post => post.IsTemporal());

            modelBuilder.Entity<Post>()
                .HasOne(post => post.PostType)
                .WithMany(postType => postType.Posts)
                .HasForeignKey(post => post.PostTypeId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Post>()
                .HasOne(post => post.Approval)
                .WithMany(approval => approval.Posts)
                .HasForeignKey(post => post.ApprovalId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
