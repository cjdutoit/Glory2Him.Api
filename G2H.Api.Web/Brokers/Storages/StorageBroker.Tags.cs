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
using G2H.Api.Web.Models.Tags;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace G2H.Api.Web.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<Tag> Tags { get; set; }

        public async ValueTask<Tag> InsertTagAsync(Tag tag)
        {
            using var broker =
                new StorageBroker(this.configuration);

            EntityEntry<Tag> tagEntityEntry =
                await broker.Tags.AddAsync(tag);

            await broker.SaveChangesAsync();

            return tagEntityEntry.Entity;
        }

        public IQueryable<Tag> SelectAllTags()
        {
            using var broker =
                new StorageBroker(this.configuration);

            return broker.Tags;
        }

        public async ValueTask<Tag> SelectTagByIdAsync(Guid tagId)
        {
            using var broker =
                new StorageBroker(this.configuration);

            return await broker.Tags.FindAsync(tagId);
        }

        public async ValueTask<Tag> UpdateTagAsync(Tag tag)
        {
            using var broker =
                new StorageBroker(this.configuration);

            EntityEntry<Tag> tagEntityEntry =
                broker.Tags.Update(tag);

            await broker.SaveChangesAsync();

            return tagEntityEntry.Entity;
        }

        public async ValueTask<Tag> DeleteTagAsync(Tag tag)
        {
            using var broker =
                new StorageBroker(this.configuration);

            EntityEntry<Tag> tagEntityEntry =
                broker.Tags.Remove(tag);

            await broker.SaveChangesAsync();

            return tagEntityEntry.Entity;
        }
    }
}
