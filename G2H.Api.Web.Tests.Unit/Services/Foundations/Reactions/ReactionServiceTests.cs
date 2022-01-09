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
using G2H.Api.Web.Models.Reactions;
using G2H.Api.Web.Models.Users;
using G2H.Api.Web.Services.Foundations.Reactions;
using Microsoft.Data.SqlClient;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;
using Xunit;

namespace G2H.Api.Web.Tests.Unit.Services.Foundations.Reactions
{
    public partial class ReactionServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IReactionService reactionService;

        public ReactionServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.reactionService = new ReactionService(
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

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static IQueryable<Reaction> CreateRandomReactions()
        {
            return CreateReactionFiller(dateTimeOffset: GetRandomDateTimeOffset())
                .Create(count: GetRandomNumber())
                    .AsQueryable();
        }

        private static Reaction CreateRandomModifyReaction(DateTimeOffset dateTimeOffset)
        {
            int randomDaysInPast = GetRandomNegativeNumber();
            Reaction randomReaction = CreateRandomReaction(dateTimeOffset);

            randomReaction.CreatedDate =
                randomReaction.CreatedDate.AddDays(randomDaysInPast);

            return randomReaction;
        }

        private static Reaction CreateRandomReaction() =>
            CreateReactionFiller(dateTimeOffset: GetRandomDateTimeOffset()).Create();

        private static Reaction CreateRandomReaction(DateTimeOffset dateTimeOffset) =>
            CreateReactionFiller(dateTimeOffset).Create();

        private static Filler<Reaction> CreateReactionFiller(DateTimeOffset dateTimeOffset)
        {
            var reactionId = GetRandomReactionId();
            var userId = Guid.NewGuid();
            var filler = new Filler<Reaction>();

            filler.Setup()
                .OnProperty(reaction => reaction.Id).Use(reactionId)
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnProperty(reaction => reaction.CreatedByUserId).Use(userId)
                .OnProperty(reaction => reaction.UpdatedByUserId).Use(userId)
                .OnProperty(reaction => reaction.PostReactions).IgnoreIt()
                .OnProperty(reaction => reaction.CommentReactions).IgnoreIt()
                .OnType<ApplicationUser>().IgnoreIt();

            return filler;
        }

        private static ReactionId GetRandomReactionId()
        {
            Array values = Enum.GetValues(typeof(ReactionId));
            Random random = new Random();
            ReactionId randomReactionId = (ReactionId)values.GetValue(random.Next(values.Length));
            return randomReactionId;
        }
    }
}
