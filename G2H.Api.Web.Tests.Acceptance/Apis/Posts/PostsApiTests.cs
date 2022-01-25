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
using G2H.Api.Web.Tests.Acceptance.Brokers;
using G2H.Api.Web.Tests.Acceptance.Models.Approvals;
using G2H.Api.Web.Tests.Acceptance.Models.Posts;
using Tynamix.ObjectFiller;
using Xunit;

namespace G2H.Api.Web.Tests.Acceptance.Apis.Posts
{
    [Collection(nameof(ApiTestCollection))]
    public partial class PostsApiTests
    {
        private readonly ApiBroker apiBroker;

        public PostsApiTests(ApiBroker apiBroker) =>
            this.apiBroker = apiBroker;

        private async ValueTask<Post> PostRandomPostAsync()
        {
            Post randomPost = await CreateRandomPost();
            await this.apiBroker.PostPostAsync(randomPost);

            return randomPost;
        }

        private async ValueTask<List<Post>> CreateRandomPostsAsync()
        {
            int randomNumber = GetRandomNumber();
            var randomPosts = new List<Post>();

            for (int i = 0; i < randomNumber; i++)
            {
                randomPosts.Add(await PostRandomPostAsync());
            }
            return randomPosts;
        }

        private static Post UpdatePostWithRandomValues(Post inputPost)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            var filler = new Filler<Post>();

            filler.Setup()
                .OnProperty(post => post.Id).Use(inputPost.Id)
                .OnProperty(post => post.ApprovalId).Use(inputPost.ApprovalId)
                .OnProperty(post => post.CreatedDate).Use(inputPost.CreatedDate)
                .OnProperty(post => post.CreatedByUserId).Use(inputPost.CreatedByUserId)
                .OnProperty(post => post.UpdatedDate).Use(now)
                .OnProperty(post => post.PostType).IgnoreIt()
                .OnProperty(post => post.Approval).IgnoreIt()
                .OnType<DateTimeOffset>().Use(GetRandomDateTime());

            return filler.Create();
        }

        private static DateTimeOffset GetRandomDateTime() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private async ValueTask<Approval> PostRandomApprovalAsync()
        {
            Approval randomApproval = CreateRandomApproval();
            await this.apiBroker.PostApprovalAsync(randomApproval);

            return randomApproval;
        }

        private static Approval CreateRandomApproval() =>
            CreateRandomApprovalFiller().Create();

        private static Filler<Approval> CreateRandomApprovalFiller()
        {
            Guid userId = Guid.NewGuid();
            DateTime now = DateTime.UtcNow;
            var filler = new Filler<Approval>();

            filler.Setup()
                .OnProperty(approval => approval.Status).IgnoreIt()
                .OnProperty(approval => approval.CreatedDate).Use(now)
                .OnProperty(approval => approval.CreatedByUserId).Use(userId)
                .OnProperty(approval => approval.UpdatedDate).Use(now)
                .OnProperty(approval => approval.UpdatedByUserId).Use(userId);

            return filler;
        }

        private async ValueTask<Post> CreateRandomPost()
        {
            Approval approval = await PostRandomApprovalAsync();
            Guid userId = Guid.NewGuid();
            DateTime now = DateTime.UtcNow;
            var filler = new Filler<Post>();

            filler.Setup()
                .OnProperty(post => post.ApprovalId).Use(approval.Id)
                .OnProperty(post => post.CreatedByUserId).Use(userId)
                .OnProperty(post => post.UpdatedByUserId).Use(userId)
                .OnProperty(post => post.CreatedDate).Use(now)
                .OnProperty(post => post.UpdatedDate).Use(now)
                .OnProperty(post => post.PostType).IgnoreIt()
                .OnProperty(post => post.Approval).IgnoreIt();

            return filler.Create();
        }
    }
}
