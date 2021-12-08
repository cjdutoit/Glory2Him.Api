// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System.Threading.Tasks;
using G2H.Api.Web.Models.Attachments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace G2H.Api.Web.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<Attachment> Attachments { get; set; }

        public async ValueTask<Attachment> InsertAttachmentAsync(Attachment attachment)
        {
            using var broker =
                new StorageBroker(this.configuration);

            EntityEntry<Attachment> attachmentEntityEntry =
                await broker.Attachments.AddAsync(attachment);

            await broker.SaveChangesAsync();

            return attachmentEntityEntry.Entity;
        }
    }
}
