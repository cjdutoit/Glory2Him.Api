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
using G2H.Api.Web.Models.Posts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace G2H.Api.Web.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<Post> Posts { get; set; }

        public async ValueTask<Post> InsertPostAsync(Post post)
        {
            using var broker =
                new StorageBroker(configuration);

            EntityEntry<Post> postEntityEntry =
                await broker.Posts.AddAsync(post);

            await broker.SaveChangesAsync();

            return postEntityEntry.Entity;
        }

        public IQueryable<Post> SelectAllPosts()
        {
            using var broker =
                new StorageBroker(configuration);

            return broker.Posts;
        }

        public async ValueTask<Post> SelectPostByIdAsync(Guid postId)
        {
            using var broker =
                new StorageBroker(configuration);

            return await broker.Posts.FindAsync(postId);
        }

        public async ValueTask<Post> UpdatePostAsync(Post post)
        {
            using var broker =
                new StorageBroker(configuration);

            EntityEntry<Post> postEntityEntry =
                broker.Posts.Update(post);

            await broker.SaveChangesAsync();

            return postEntityEntry.Entity;
        }

        public async ValueTask<Post> DeletePostAsync(Post post)
        {
            using var broker =
                new StorageBroker(configuration);

            EntityEntry<Post> postEntityEntry =
                broker.Posts.Remove(post);

            await broker.SaveChangesAsync();

            return postEntityEntry.Entity;
        }
    }
}
