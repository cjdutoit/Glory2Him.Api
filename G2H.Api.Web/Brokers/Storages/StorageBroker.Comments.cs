// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using G2H.Api.Web.Models.Comments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace G2H.Api.Web.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<Comment> Comments { get; set; }

        public async ValueTask<Comment> InsertCommentAsync(Comment comment)
        {
            using var broker =
                new StorageBroker(this.configuration);

            EntityEntry<Comment> commentEntityEntry =
                await broker.Comments.AddAsync(comment);

            await broker.SaveChangesAsync();

            return commentEntityEntry.Entity;
        }

        public IQueryable<Comment> SelectAllComments()
        {
            using var broker =
                new StorageBroker(this.configuration);

            return broker.Comments;
        }

        public async ValueTask<Comment> SelectCommentByIdAsync(Guid commentId)
        {
            using var broker =
                new StorageBroker(this.configuration);

            return await broker.Comments.FindAsync(commentId);
        }
    }
}
