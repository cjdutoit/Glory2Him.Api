﻿// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using G2H.Api.Web.Models.PostTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace G2H.Api.Web.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<PostType> PostTypes { get; set; }

        public async ValueTask<PostType> InsertPostTypeAsync(PostType postType)
        {
            using var broker =
                new StorageBroker(this.configuration);

            EntityEntry<PostType> postTypeEntityEntry =
                await broker.PostTypes.AddAsync(postType);

            await broker.SaveChangesAsync();

            return postTypeEntityEntry.Entity;
        }

        public IQueryable<PostType> SelectAllPostTypes()
        {
            using var broker =
                new StorageBroker(this.configuration);

            return broker.PostTypes;
        }

        public async ValueTask<PostType> SelectPostTypeByIdAsync(PostTypeId postTypeId)
        {
            using var broker =
                new StorageBroker(this.configuration);

            return await broker.PostTypes.FindAsync(postTypeId);
        }

        public async ValueTask<PostType> UpdatePostTypeAsync(PostType postType)
        {
            using var broker =
                new StorageBroker(this.configuration);

            EntityEntry<PostType> postTypeEntityEntry =
                broker.PostTypes.Update(postType);

            await broker.SaveChangesAsync();

            return postTypeEntityEntry.Entity;
        }

        public async ValueTask<PostType> DeletePostTypeAsync(PostType postType)
        {
            using var broker =
                new StorageBroker(this.configuration);

            EntityEntry<PostType> postTypeEntityEntry =
                broker.PostTypes.Remove(postType);

            await broker.SaveChangesAsync();

            return postTypeEntityEntry.Entity;
        }
    }
}
