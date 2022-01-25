// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using G2H.Api.Web.Tests.Acceptance.Models.Posts;
using Xunit;

namespace G2H.Api.Web.Tests.Acceptance.Apis.Posts
{
    public partial class PostsApiTests
    {
        [Fact]
        public async Task ShouldPostPostAsync()
        {
            // given
            Post randomPost = await CreateRandomPost();
            Post inputPost = randomPost;
            Post expectedPost = inputPost;

            // when 
            await this.apiBroker.PostPostAsync(inputPost);

            Post actualPost =
                await this.apiBroker.GetPostByIdAsync(inputPost.Id);

            // then
            actualPost.Should().BeEquivalentTo(expectedPost);
            await this.apiBroker.DeletePostByIdAsync(actualPost.Id);
            await this.apiBroker.DeleteApprovalByIdAsync(actualPost.ApprovalId);
        }

        [Fact]
        public async Task ShouldGetAllPostsAsync()
        {
            // given
            List<Post> randomPosts = await CreateRandomPostsAsync();
            List<Post> expectedPosts = randomPosts;

            // when
            List<Post> actualPosts = await this.apiBroker.GetAllPostsAsync();

            // then
            foreach (Post expectedPost in expectedPosts)
            {
                Post actualPost = actualPosts.Single(post => post.Id == expectedPost.Id);
                actualPost.Should().BeEquivalentTo(expectedPost);
                await this.apiBroker.DeletePostByIdAsync(actualPost.Id);
                await this.apiBroker.DeleteApprovalByIdAsync(actualPost.ApprovalId);
            }
        }

        [Fact]
        public async Task ShouldGetPostAsync()
        {
            // given
            Post randomPost = await PostRandomPostAsync();
            Post expectedPost = randomPost;

            // when
            Post actualPost = await this.apiBroker.GetPostByIdAsync(randomPost.Id);

            // then
            actualPost.Should().BeEquivalentTo(expectedPost);
            await this.apiBroker.DeletePostByIdAsync(actualPost.Id);
            await this.apiBroker.DeleteApprovalByIdAsync(actualPost.ApprovalId);
        }
    }
}
