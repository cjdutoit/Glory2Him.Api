// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using G2H.Api.Web.Models.CommentReactions;
using Microsoft.EntityFrameworkCore;

namespace G2H.Api.Web.Brokers.Storages
{
    public partial class StorageBroker
    {
        private static void AddCommentReactionReferences(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CommentReaction>()
                .HasKey(commentReaction =>
                    new
                    {
                        commentReaction.CommentId,
                        commentReaction.ReactionId
                    });

            modelBuilder.Entity<CommentReaction>()
                .HasOne(commentReaction => commentReaction.Comment)
                .WithMany(comment => comment.CommentReactions)
                .HasForeignKey(commentReaction => commentReaction.CommentId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<CommentReaction>()
                .HasOne(commentReaction => commentReaction.Reaction)
                .WithMany(reaction => reaction.CommentReactions)
                .HasForeignKey(commentReaction => commentReaction.ReactionId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
