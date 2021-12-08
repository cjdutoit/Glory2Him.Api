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
using G2H.Api.Web.Models.PostTags;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace G2H.Api.Web.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<PostTag> PostTags { get; set; }

        public async ValueTask<PostTag> InsertPostTagAsync(PostTag postTag)
        {
            using var broker =
                new StorageBroker(this.configuration);

            EntityEntry<PostTag> postTagEntityEntry =
                await broker.PostTags.AddAsync(postTag);

            await broker.SaveChangesAsync();

            return postTagEntityEntry.Entity;
        }

        public IQueryable<PostTag> SelectAllPostTags()
        {
            using var broker =
                new StorageBroker(this.configuration);

            return broker.PostTags;
        }

        public async ValueTask<PostTag> SelectPostTagByIdAsync(Guid postTagId)
        {
            using var broker =
                new StorageBroker(this.configuration);

            return await broker.PostTags.FindAsync(postTagId);
        }

        public async ValueTask<PostTag> UpdatePostTagAsync(PostTag postTag)
        {
            using var broker =
                new StorageBroker(this.configuration);

            EntityEntry<PostTag> postTagEntityEntry =
                broker.PostTags.Update(postTag);

            await broker.SaveChangesAsync();

            return postTagEntityEntry.Entity;
        }

        public async ValueTask<PostTag> DeletePostTagAsync(PostTag postTag)
        {
            using var broker =
                new StorageBroker(this.configuration);

            EntityEntry<PostTag> postTagEntityEntry =
                broker.PostTags.Remove(postTag);

            await broker.SaveChangesAsync();

            return postTagEntityEntry.Entity;
        }
    }
}
