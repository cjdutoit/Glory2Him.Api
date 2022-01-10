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
using G2H.Api.Web.Models.Users;
using G2H.Api.Web.Services.Foundations.Approvals;
using Microsoft.Data.SqlClient;
using Moq;
using Tynamix.ObjectFiller;
using Xeptions;
using Xunit;

namespace G2H.Api.Web.Tests.Unit.Services.Foundations.Approvals
{
    public partial class ApprovalServiceTests
    {
        private readonly Mock<IStorageBroker> storageBrokerMock;
        private readonly Mock<IDateTimeBroker> dateTimeBrokerMock;
        private readonly Mock<ILoggingBroker> loggingBrokerMock;
        private readonly IApprovalService approvalService;

        public ApprovalServiceTests()
        {
            this.storageBrokerMock = new Mock<IStorageBroker>();
            this.dateTimeBrokerMock = new Mock<IDateTimeBroker>();
            this.loggingBrokerMock = new Mock<ILoggingBroker>();

            this.approvalService = new ApprovalService(
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

        private static SqlException GetSqlException() =>
            (SqlException)FormatterServices.GetUninitializedObject(typeof(SqlException));

        private static Approval CreateRandomApproval() =>
            CreateApprovalFiller(dateTimeOffset: GetRandomDateTimeOffset()).Create();

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

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static IQueryable<Approval> CreateRandomApprovals()
        {
            return CreateApprovalFiller(dateTimeOffset: GetRandomDateTimeOffset())
                .Create(count: GetRandomNumber())
                    .AsQueryable();
        }

        private static Approval CreateRandomApproval(DateTimeOffset dateTimeOffset) =>
            CreateApprovalFiller(dateTimeOffset).Create();

        private static Approval CreateRandomModifyApproval(DateTimeOffset dateTimeOffset)
        {
            int randomDaysInPast = GetRandomNegativeNumber();
            Approval randomApproval = CreateRandomApproval(dateTimeOffset);

            randomApproval.CreatedDate =
                randomApproval.CreatedDate.AddDays(randomDaysInPast);

            return randomApproval;
        }

        private static Filler<Approval> CreateApprovalFiller(DateTimeOffset dateTimeOffset)
        {
            var userId = Guid.NewGuid();
            var filler = new Filler<Approval>();

            filler.Setup()
                .OnType<DateTimeOffset>().Use(dateTimeOffset)
                .OnProperty(approval => approval.CreatedByUserId).Use(userId)
                .OnProperty(approval => approval.UpdatedByUserId).Use(userId)
                .OnProperty(approval => approval.Posts).IgnoreIt()
                .OnProperty(approval => approval.Comments).IgnoreIt()
                .OnProperty(approval => approval.Attachments).IgnoreIt()
                .OnProperty(approval => approval.PostTags).IgnoreIt()
                .OnProperty(approval => approval.Tags).IgnoreIt()
                .OnProperty(approval => approval.ApprovalUsers).IgnoreIt()
                .OnType<ApplicationUser>().IgnoreIt();

            return filler;
        }
    }
}
