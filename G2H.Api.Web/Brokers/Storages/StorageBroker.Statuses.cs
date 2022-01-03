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
using G2H.Api.Web.Models.Statuses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace G2H.Api.Web.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<Status> Statuses { get; set; }

        public async ValueTask<Status> InsertStatusAsync(Status status)
        {
            using var broker =
                new StorageBroker(this.configuration);

            EntityEntry<Status> statusEntityEntry =
                await broker.Statuses.AddAsync(status);

            await broker.SaveChangesAsync();

            return statusEntityEntry.Entity;
        }

        public IQueryable<Status> SelectAllStatuses()
        {
            using var broker =
                new StorageBroker(this.configuration);

            return broker.Statuses;
        }

        public async ValueTask<Status> SelectStatusByIdAsync(Guid statusId)
        {
            using var broker =
                new StorageBroker(this.configuration);

            return await broker.Statuses.FindAsync(statusId);
        }

        public async ValueTask<Status> UpdateStatusAsync(Status status)
        {
            using var broker =
                new StorageBroker(this.configuration);

            EntityEntry<Status> statusEntityEntry =
                broker.Statuses.Update(status);

            await broker.SaveChangesAsync();

            return statusEntityEntry.Entity;
        }

        public async ValueTask<Status> DeleteStatusAsync(Status status)
        {
            using var broker =
                new StorageBroker(this.configuration);

            EntityEntry<Status> statusEntityEntry =
                broker.Statuses.Remove(status);

            await broker.SaveChangesAsync();

            return statusEntityEntry.Entity;
        }
    }
}
