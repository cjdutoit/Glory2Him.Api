// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using G2H.Api.Web.Models.PostReactions;
using Microsoft.EntityFrameworkCore;

namespace G2H.Api.Web.Brokers.Storages
{
    public partial class StorageBroker
    {
        private static void AddPostReactionReferences(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PostReaction>()
                .HasOne(postReaction => postReaction.Post)
                .WithMany(post => post.PostReactions)
                .HasForeignKey(postReaction => postReaction.PostId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PostReaction>()
                .HasOne(postReaction => postReaction.Reaction)
                .WithMany(reaction => reaction.PostReactions)
                .HasForeignKey(postReaction => postReaction.ReactionId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
