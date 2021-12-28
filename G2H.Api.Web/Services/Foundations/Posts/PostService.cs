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
using G2H.Api.Web.Brokers.DateTimes;
using G2H.Api.Web.Brokers.Loggings;
using G2H.Api.Web.Brokers.Storages;
using G2H.Api.Web.Models.Posts;

namespace G2H.Api.Web.Services.Foundations.Posts
{
    public partial class PostService : IPostService
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public PostService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<Post> AddPostAsync(Post post) =>
        TryCatch(async () =>
        {
            ValidatePostOnAdd(post);

            return await this.storageBroker.InsertPostAsync(post);
        });

        public IQueryable<Post> RetrieveAllPosts() =>
        TryCatch(() => this.storageBroker.SelectAllPosts());

        public ValueTask<Post> RetrievePostByIdAsync(Guid postId) =>
        TryCatch(async () =>
        {
            Post maybePost = await this.storageBroker
                .SelectPostByIdAsync(postId);

            return maybePost;
        });
    }
}
