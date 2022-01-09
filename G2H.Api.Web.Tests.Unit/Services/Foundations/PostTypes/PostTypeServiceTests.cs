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
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using G2H.Api.Web.Brokers.DateTimes;
using G2H.Api.Web.Brokers.Loggings;
using G2H.Api.Web.Brokers.Storages;
using G2H.Api.Web.Models.PostTypes;
using G2H.Api.Web.Models.Users;
using G2H.Api.Web.Services.Foundations.PostTypes;
using Microsoft.Data.SqlClient;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;
using Xunit;

namespace G2H.Api.Web.Tests.Unit.Services.Foundations.PostTypes
{
    public partial class PostTypeServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IPostTypeService postTypeService;

        public PostTypeServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.postTypeService = new PostTypeService(
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

        private static IQueryable<PostType> CreateRandomPostTypes()
        {
            return CreatePostTypeFiller(dateTimeOffset: GetRandomDateTimeOffset())
                .Create(count: GetRandomNumber())
                    .AsQueryable();
        }

        private static PostType CreateRandomModifyPostType(DateTimeOffset dateTimeOffset)
        {
            int randomDaysInPast = GetRandomNegativeNumber();
            PostType randomPostType = CreateRandomPostType(dateTimeOffset);

            randomPostType.CreatedDate =
                randomPostType.CreatedDate.AddDays(randomDaysInPast);

            return randomPostType;
        }

        private static string GetRandomMessage() =>
            new MnemonicString(wordCount: GetRandomNumber()).GetValue();

        private static SqlException GetSqlException() =>
            (SqlException)FormatterServices.GetUninitializedObject(typeof(SqlException));

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

        public static IEnumerable<object[]> InvalidMinuteCases()
        {
            int randomMoreThanMinuteFromNow = GetRandomNumber();
            int randomMoreThanMinuteBeforeNow = GetRandomNegativeNumber();

            return new List<object[]>
            {
                new object[] { randomMoreThanMinuteFromNow },
                new object[] { randomMoreThanMinuteBeforeNow }
            };
        }

        private static PostType CreateRandomPostType() =>
            CreatePostTypeFiller(dateTimeOffset: GetRandomDateTimeOffset()).Create();

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static PostType CreateRandomPostType(DateTimeOffset dateTimeOffset) =>
            CreatePostTypeFiller(dateTimeOffset).Create();

        private static Filler<PostType> CreatePostTypeFiller(DateTimeOffset dateTimeOffset)
        {
            var userId = Guid.NewGuid();
            var filler = new Filler<PostType>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnProperty(postType => postType.CreatedByUserId).Use(userId)
                .OnProperty(postType => postType.UpdatedByUserId).Use(userId)
                .OnType<ApplicationUser>().IgnoreIt()
                .OnProperty(status => status.Posts).IgnoreIt();

            return filler;
        }

        private static PostTypeId GetRandomPostTypeId()
        {
            Array values = Enum.GetValues(typeof(PostTypeId));
            Random random = new Random();
            PostTypeId randomPostTypeId = (PostTypeId)values.GetValue(random.Next(values.Length));
            return randomPostTypeId;
        }
    }
}
