﻿// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

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
    }
}
