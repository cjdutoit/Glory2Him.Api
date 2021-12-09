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
using G2H.Api.Web.Models.PostsComments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace G2H.Api.Web.Brokers.Storages
{
    public partial class StorageBroker
    {
        public DbSet<PostAttachment> PostAttachments { get; set; }

        public async ValueTask<PostAttachment> InsertPostAttachmentAsync(PostAttachment postAttachment)
        {
            using var broker =
                new StorageBroker(this.configuration);

            EntityEntry<PostAttachment> postAttachmentEntityEntry =
                await broker.PostAttachments.AddAsync(postAttachment);

            await broker.SaveChangesAsync();

            return postAttachmentEntityEntry.Entity;
        }

        public IQueryable<PostAttachment> SelectAllPostAttachments()
        {
            using var broker =
                new StorageBroker(this.configuration);

            return broker.PostAttachments;
        }

        public async ValueTask<PostAttachment> SelectPostAttachmentByIdAsync(Guid postAttachmentId)
        {
            using var broker =
                new StorageBroker(this.configuration);

            return await broker.PostAttachments.FindAsync(postAttachmentId);
        }

        public async ValueTask<PostAttachment> UpdatePostAttachmentAsync(PostAttachment postAttachment)
        {
            using var broker =
                new StorageBroker(this.configuration);

            EntityEntry<PostAttachment> postAttachmentEntityEntry =
                broker.PostAttachments.Update(postAttachment);

            await broker.SaveChangesAsync();

            return postAttachmentEntityEntry.Entity;
        }

        public async ValueTask<PostAttachment> DeletePostAttachmentAsync(PostAttachment postAttachment)
        {
            using var broker =
                new StorageBroker(this.configuration);

            EntityEntry<PostAttachment> postAttachmentEntityEntry =
                broker.PostAttachments.Remove(postAttachment);

            await broker.SaveChangesAsync();

            return postAttachmentEntityEntry.Entity;
        }
    }
}
