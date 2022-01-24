// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using G2H.Api.Web.Tests.Acceptance.Models.Posts;

namespace G2H.Api.Web.Tests.Acceptance.Brokers
{
    public partial class ApiBroker
    {
        private const string PostsRelativeUrl = "api/posts";

        public async ValueTask<Post> PostPostAsync(Post post) =>
            await this.apiFactoryClient.PostContentAsync(PostsRelativeUrl, post);

        public async ValueTask<Post> GetPostByIdAsync(Guid postId) =>
            await this.apiFactoryClient.GetContentAsync<Post>($"{PostsRelativeUrl}/{postId}");

        public async ValueTask<List<Post>> GetAllPostsAsync() =>
          await this.apiFactoryClient.GetContentAsync<List<Post>>($"{PostsRelativeUrl}/");

        public async ValueTask<Post> PutPostAsync(Post post) =>
            await this.apiFactoryClient.PutContentAsync(PostsRelativeUrl, post);

        public async ValueTask<Post> DeletePostByIdAsync(Guid postId) =>
            await this.apiFactoryClient.DeleteContentAsync<Post>($"{PostsRelativeUrl}/{postId}");
    }
}
