// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System.Linq;
using FluentAssertions;
using G2H.Api.Web.Models.Posts;
using Moq;
using Xunit;

namespace G2H.Api.Web.Tests.Unit.Services.Foundations.Posts
{
    public partial class PostServiceTests
    {
        [Fact]
        public void ShouldReturnPosts()
        {
            // given
            IQueryable<Post> randomPosts = CreateRandomPosts();
            IQueryable<Post> storagePosts = randomPosts;
            IQueryable<Post> expectedPosts = storagePosts;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllPosts())
                    .Returns(storagePosts);

            // when
            IQueryable<Post> actualPosts =
                this.postService.RetrieveAllPosts();

            // then
            actualPosts.Should().BeEquivalentTo(expectedPosts);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllPosts(),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
