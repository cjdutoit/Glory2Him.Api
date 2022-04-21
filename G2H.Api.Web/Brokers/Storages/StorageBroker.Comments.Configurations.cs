﻿// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using G2H.Api.Web.Models.Comments;
using Microsoft.EntityFrameworkCore;

namespace G2H.Api.Web.Brokers.Storages
{
    public partial class StorageBroker
    {
        private static void AddCommentConfigurations(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Comment>()
                .ToTable(post => post.IsTemporal());

            modelBuilder.Entity<Comment>()
                .HasOne(comment => comment.Approval)
                .WithMany(approval => approval.Comments)
                .HasForeignKey(comment => comment.ApprovalId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
