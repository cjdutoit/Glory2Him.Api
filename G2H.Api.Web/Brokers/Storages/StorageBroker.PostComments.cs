﻿// --------------------------------------------------------------------------------
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
using G2H.Api.Web.Models.PostComments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace G2H.Api.Web.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<PostComment> PostComments { get; set; }

        public async ValueTask<PostComment> InsertPostCommentAsync(PostComment postComment)
        {
            using var broker =
                new StorageBroker(this.configuration);

            EntityEntry<PostComment> postCommentEntityEntry =
                await broker.PostComments.AddAsync(postComment);

            await broker.SaveChangesAsync();

            return postCommentEntityEntry.Entity;
        }

        public IQueryable<PostComment> SelectAllPostComments()
        {
            using var broker =
                new StorageBroker(this.configuration);

            return broker.PostComments;
        }

        public async ValueTask<PostComment> SelectPostCommentByIdAsync(Guid postCommentId)
        {
            using var broker =
                new StorageBroker(this.configuration);

            return await broker.PostComments.FindAsync(postCommentId);
        }

        public async ValueTask<PostComment> UpdatePostCommentAsync(PostComment postComment)
        {
            using var broker =
                new StorageBroker(this.configuration);

            EntityEntry<PostComment> postCommentEntityEntry =
                broker.PostComments.Update(postComment);

            await broker.SaveChangesAsync();

            return postCommentEntityEntry.Entity;
        }
        public async ValueTask<PostComment> DeletePostCommentAsync(PostComment postComment)
        {
            using var broker =
                new StorageBroker(this.configuration);

            EntityEntry<PostComment> postCommentEntityEntry =
                broker.PostComments.Remove(postComment);

            await broker.SaveChangesAsync();

            return postCommentEntityEntry.Entity;
        }
    }
}
