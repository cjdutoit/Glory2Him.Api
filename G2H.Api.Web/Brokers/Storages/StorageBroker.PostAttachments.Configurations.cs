﻿// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using G2H.Api.Web.Models.PostAttachments;
using Microsoft.EntityFrameworkCore;

namespace G2H.Api.Web.Brokers.Storages
{
    public partial class StorageBroker
    {
        private static void AddPostAttachmentConfigurations(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PostAttachment>()
                .HasKey(postAttachment =>
                    new
                    {
                        postAttachment.PostId,
                        postAttachment.AttachmentId
                    });

            modelBuilder.Entity<PostAttachment>()
                .HasOne(postAttachment => postAttachment.Post)
                .WithMany(post => post.PostAttachments)
                .HasForeignKey(postAttachment => postAttachment.PostId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PostAttachment>()
                .HasOne(postAttachment => postAttachment.Attachment)
                .WithMany(attachment => attachment.PostAttachments)
                .HasForeignKey(postAttachment => postAttachment.AttachmentId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
