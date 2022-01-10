// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using G2H.Api.Web.Models.Approvals;
using G2H.Api.Web.Models.Approvals.Exceptions;
using Moq;
using Xunit;

namespace G2H.Api.Web.Tests.Unit.Services.Foundations.Approvals
{
    public partial class ApprovalServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfApprovalIsNullAndLogItAsync()
        {
            // given
            Approval nullApproval = null;

            var nullApprovalException =
                new NullApprovalException();

            var expectedApprovalValidationException =
                new ApprovalValidationException(nullApprovalException);

            // when
            ValueTask<Approval> addApprovalTask =
                this.approvalService.AddApprovalAsync(nullApproval);

            // then
            await Assert.ThrowsAsync<ApprovalValidationException>(() =>
                addApprovalTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedApprovalValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfApprovalIsInvalidAndLogItAsync()
        {
            // given
            var invalidApproval = new Approval();

            var invalidApprovalException =
                new InvalidApprovalException();

            invalidApprovalException.AddData(
                key: nameof(Approval.Id),
                values: "Id is required");

            invalidApprovalException.AddData(
                key: nameof(Approval.StatusId),
                values: "Id is required");

            invalidApprovalException.AddData(
                key: nameof(Approval.CreatedDate),
                values: "Date is required");

            invalidApprovalException.AddData(
                key: nameof(Approval.CreatedByUserId),
                values: "Id is required");

            invalidApprovalException.AddData(
                key: nameof(Approval.UpdatedDate),
                values: "Date is required");

            invalidApprovalException.AddData(
                key: nameof(Approval.UpdatedByUserId),
                values: "Id is required");

            var expectedApprovalValidationException =
                new ApprovalValidationException(invalidApprovalException);

            // when
            ValueTask<Approval> addApprovalTask =
                this.approvalService.AddApprovalAsync(invalidApproval);

            // then
            var ex = await Assert.ThrowsAsync<ApprovalValidationException>(() =>
               addApprovalTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedApprovalValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertApprovalAsync(It.IsAny<Approval>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCreateAndUpdateDatesIsNotSameAndLogItAsync()
        {
            // given
            int randomNumber = GetRandomNumber();
            Approval randomApproval = CreateRandomApproval();
            Approval invalidApproval = randomApproval;

            invalidApproval.UpdatedDate =
                invalidApproval.CreatedDate.AddDays(randomNumber);

            var invalidApprovalException =
                new InvalidApprovalException();

            invalidApprovalException.AddData(
                key: nameof(Approval.UpdatedDate),
                values: $"Date is not the same as {nameof(Approval.CreatedDate)}");

            var expectedApprovalValidationException =
                new ApprovalValidationException(invalidApprovalException);

            // when
            ValueTask<Approval> addApprovalTask =
                this.approvalService.AddApprovalAsync(invalidApproval);

            // then
            await Assert.ThrowsAsync<ApprovalValidationException>(() =>
               addApprovalTask.AsTask());


            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedApprovalValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertApprovalAsync(It.IsAny<Approval>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCreateAndUpdateUserIdsIsNotSameAndLogItAsync()
        {
            // given
            int randomNumber = GetRandomNumber();
            Approval randomApproval = CreateRandomApproval();
            Approval invalidApproval = randomApproval;

            invalidApproval.UpdatedByUserId = Guid.NewGuid();

            var invalidApprovalException =
                new InvalidApprovalException();

            invalidApprovalException.AddData(
                key: nameof(Approval.UpdatedByUserId),
                values: $"Id is not the same as {nameof(Approval.CreatedByUserId)}");

            var expectedApprovalValidationException =
                new ApprovalValidationException(invalidApprovalException);

            // when
            ValueTask<Approval> addApprovalTask =
                this.approvalService.AddApprovalAsync(invalidApproval);

            // then
            await Assert.ThrowsAsync<ApprovalValidationException>(() =>
               addApprovalTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedApprovalValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertApprovalAsync(It.IsAny<Approval>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(MinutesBeforeOrAfter))]
        public async Task ShouldThrowValidationExceptionOnAddIfCreatedDateIsNotRecentAndLogItAsync(
            int minutesBeforeOrAfter)
        {
            // given
            DateTimeOffset randomDateTimeOffset =
                GetRandomDateTimeOffset();

            DateTimeOffset invalidDateTime =
                randomDateTimeOffset.AddMinutes(minutesBeforeOrAfter);

            Approval randomApproval = CreateRandomApproval(invalidDateTime);
            Approval invalidApproval = randomApproval;

            var invalidApprovalException =
                new InvalidApprovalException();

            invalidApprovalException.AddData(
                key: nameof(Approval.CreatedDate),
                values: "Date is not recent");

            var expectedApprovalValidationException =
                new ApprovalValidationException(invalidApprovalException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTimeOffset);

            // when
            ValueTask<Approval> addApprovalTask =
                this.approvalService.AddApprovalAsync(invalidApproval);

            // then
            await Assert.ThrowsAsync<ApprovalValidationException>(() =>
               addApprovalTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedApprovalValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertApprovalAsync(It.IsAny<Approval>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
