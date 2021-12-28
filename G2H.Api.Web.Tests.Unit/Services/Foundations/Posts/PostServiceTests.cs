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
using System.Linq.Expressions;
using System.Runtime.Serialization;
using G2H.Api.Web.Brokers.DateTimes;
using G2H.Api.Web.Brokers.Loggings;
using G2H.Api.Web.Brokers.Storages;
using G2H.Api.Web.Models.Approvals;
using G2H.Api.Web.Models.Posts;
using G2H.Api.Web.Models.PostTypes;
using G2H.Api.Web.Models.Users;
using G2H.Api.Web.Services.Foundations.Posts;
using Microsoft.Data.SqlClient;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;
using Xunit;

namespace G2H.Api.Web.Tests.Unit.Services.Foundations.Posts
{
    public partial class PostServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IPostService postService;

        public PostServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.postService = new PostService(
                storageBroker: this.storageBrokerMock.Object,
                dateTimeBroker: this.dateTimeBrokerMock.Object,
                loggingBroker: this.loggingBrokerMock.Object);
        }

        private static Expression<Func<Xeption, bool>> SameExceptionAs(Xeption expectedException)
        {
            return actualException =>
                actualException.Message == expectedException.Message
                && actualException.InnerException.Message == expectedException.InnerException.Message
                && (actualException.InnerException as Xeption).DataEquals(expectedException.InnerException.Data);
        }

        private static int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static int GetRandomNegativeNumber() =>
            -1 * new IntRange(min: 2, max: 10).GetValue();

        public static TheoryData MinutesBeforeOrAfter()
        {
            int randomNumber = GetRandomNumber();
            int randomNegativeNumber = GetRandomNegativeNumber();

            return new TheoryData<int>
            {
                randomNumber,
                randomNegativeNumber
            };
        }

        private static SqlException GetSqlException() =>
            (SqlException)FormatterServices.GetUninitializedObject(typeof(SqlException));

        private static string GetRandomMessage() =>
            new MnemonicString(wordCount: GetRandomNumber()).GetValue();

        private static string GetRandomMessage(int wordCount, int wordMinLength, int wordMaxLength) =>
            new MnemonicString(
                wordCount,
                wordMinLength,
                wordMaxLength).GetValue();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static Post CreateRandomModifyPost(DateTimeOffset dateTimeOffset)
        {
            int randomDaysInPast = GetRandomNegativeNumber();
            Post randomPost = CreateRandomPost(dateTimeOffset);

            randomPost.CreatedDate =
                randomPost.CreatedDate.AddDays(randomDaysInPast);

            return randomPost;
        }

        private static IQueryable<Post> CreateRandomPosts()
        {
            return CreatePostFiller(dateTimeOffset: GetRandomDateTimeOffset())
                .Create(count: GetRandomNumber())
                    .AsQueryable();
        }

        private static Post CreateRandomPost() =>
            CreatePostFiller(dateTimeOffset: GetRandomDateTimeOffset()).Create();

        private static Post CreateRandomPost(DateTimeOffset dateTimeOffset) =>
            CreatePostFiller(dateTimeOffset).Create();

        private static Filler<Post> CreatePostFiller(DateTimeOffset dateTimeOffset)
        {
            var userId = Guid.NewGuid();
            var filler = new Filler<Post>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<PostType>().IgnoreIt()
                .OnType<Approval>().IgnoreIt()
                .OnType<ApplicationUser>().IgnoreIt()
                .OnProperty(post => post.CreatedByUserId).Use(userId)
                .OnProperty(post => post.UpdatedByUserId).Use(userId)
                .OnProperty(post => post.PostReactions).IgnoreIt()
                .OnProperty(post => post.PostTags).IgnoreIt()
                .OnProperty(post => post.PostComments).IgnoreIt()
                .OnProperty(post => post.PostAttachments).IgnoreIt();

            return filler;
        }
    }
}
