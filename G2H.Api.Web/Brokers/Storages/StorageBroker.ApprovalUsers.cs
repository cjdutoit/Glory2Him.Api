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
using G2H.Api.Web.Models.ApprovalUsers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace G2H.Api.Web.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<ApprovalUser> ApprovalUsers { get; set; }

        public async ValueTask<ApprovalUser> InsertApprovalUserAsync(ApprovalUser approvalUser)
        {
            using var broker =
                new StorageBroker(this.configuration);

            EntityEntry<ApprovalUser> approvalUserEntityEntry =
                await broker.ApprovalUsers.AddAsync(approvalUser);

            await broker.SaveChangesAsync();

            return approvalUserEntityEntry.Entity;
        }

        public IQueryable<ApprovalUser> SelectAllApprovalUsers()
        {
            using var broker =
                new StorageBroker(this.configuration);

            return broker.ApprovalUsers;
        }

        public async ValueTask<ApprovalUser> SelectApprovalUserByIdAsync(Guid approvalUserId)
        {
            using var broker =
                new StorageBroker(this.configuration);

            return await broker.ApprovalUsers.FindAsync(approvalUserId);
        }

        public async ValueTask<ApprovalUser> UpdateApprovalUserAsync(ApprovalUser approvalUser)
        {
            using var broker =
                new StorageBroker(this.configuration);

            EntityEntry<ApprovalUser> approvalUserEntityEntry =
                broker.ApprovalUsers.Update(approvalUser);

            await broker.SaveChangesAsync();

            return approvalUserEntityEntry.Entity;
        }
    }
}
