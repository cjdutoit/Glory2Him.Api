// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System;
using G2H.Api.Web.Brokers.DateTimes;
using G2H.Api.Web.Brokers.Loggings;
using G2H.Api.Web.Brokers.Storages;
using G2H.Api.Web.Models.Statuses;
using G2H.Api.Web.Models.Users;
using G2H.Api.Web.Services.Foundations.Statuses;
using Moq;
using Tynamix.ObjectFiller;

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

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

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
