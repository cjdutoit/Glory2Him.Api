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
using G2H.Api.Web.Models.Statuses;
using G2H.Api.Web.Models.Users;
using G2H.Api.Web.Services.Foundations.Statuses;
using Microsoft.Data.SqlClient;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;
using Xunit;

namespace G2H.Api.Web.Tests.Unit.Services.Foundations.Statuses
{
    public partial class StatusServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IStatusService statusService;

        public StatusServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.statusService = new StatusService(
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

        private static IQueryable<Status> CreateRandomStatuses()
        {
            return CreateStatusFiller(dateTimeOffset: GetRandomDateTimeOffset())
                .Create(count: GetRandomNumber())
                    .AsQueryable();
        }

        private static Status CreateRandomModifyStatus(DateTimeOffset dateTimeOffset)
        {
            int randomDaysInPast = GetRandomNegativeNumber();
            Status randomStatus = CreateRandomStatus(dateTimeOffset);

            randomStatus.CreatedDate =
                randomStatus.CreatedDate.AddDays(randomDaysInPast);

            return randomStatus;
        }

        private static SqlException GetSqlException() =>
            (SqlException)FormatterServices.GetUninitializedObject(typeof(SqlException));

        private static string GetRandomMessage() =>
            new MnemonicString(wordCount: GetRandomNumber()).GetValue();

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

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static Status CreateRandomStatus() =>
            CreateStatusFiller(dateTimeOffset: GetRandomDateTimeOffset()).Create();

        private static Status CreateRandomStatus(DateTimeOffset dateTimeOffset) =>
            CreateStatusFiller(dateTimeOffset).Create();

        private static Filler<Status> CreateStatusFiller(DateTimeOffset dateTimeOffset)
        {
            var statusId = GetRandomStatusId();
            var userId = Guid.NewGuid();
            var filler = new Filler<Status>();

            filler.Setup()
                .OnProperty(status => status.Id).Use(statusId)
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnType<ApplicationUser>().IgnoreIt()
                .OnProperty(status => status.CreatedByUserId).Use(userId)
                .OnProperty(status => status.UpdatedByUserId).Use(userId)
                .OnProperty(status => status.Approvals).IgnoreIt();

            return filler;
        }

        private static StatusId GetRandomStatusId()
        {
            Array values = Enum.GetValues(typeof(StatusId));
            Random random = new Random();
            StatusId randomStatusId = (StatusId)values.GetValue(random.Next(values.Length));
            return randomStatusId;
        }
    }
}
