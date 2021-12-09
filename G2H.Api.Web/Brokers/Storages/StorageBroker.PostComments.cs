// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System.Threading.Tasks;
using G2H.Api.Web.Models.PostsComments;
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
    }
}
