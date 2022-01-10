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
using Force.DeepCloner;
using G2H.Api.Web.Models.Approvals;
using G2H.Api.Web.Models.Approvals.Exceptions;
using Moq;
using Xunit;

namespace G2H.Api.Web.Tests.Unit.Services.Foundations.Approvals
{
    public partial class ApprovalServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfApprovalIsNullAndLogItAsync()
        {
            // given
            Approval nullApproval = null;
            var nullApprovalException = new NullApprovalException();

            var expectedApprovalValidationException =
                new ApprovalValidationException(nullApprovalException);

            // when
            ValueTask<Approval> modifyApprovalTask =
                this.approvalService.ModifyApprovalAsync(nullApproval);

            // then
            await Assert.ThrowsAsync<ApprovalValidationException>(() =>
                modifyApprovalTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedApprovalValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateApprovalAsync(It.IsAny<Approval>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfApprovalIsInvalidAndLogItAsync()
        {
            // given 
            var invalidApproval = new Approval();

            var invalidApprovalException = new InvalidApprovalException();

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
                values:
                new[] {
                    "Date is required",
                    $"Date is the same as {nameof(Approval.CreatedDate)}"
                });

            invalidApprovalException.AddData(
                key: nameof(Approval.UpdatedByUserId),
                values: "Id is required");

            var expectedApprovalValidationException =
                new ApprovalValidationException(invalidApprovalException);

            // when
            ValueTask<Approval> modifyApprovalTask =
                this.approvalService.ModifyApprovalAsync(invalidApproval);

            //then
            await Assert.ThrowsAsync<ApprovalValidationException>(() =>
                modifyApprovalTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedApprovalValidationException))),
                        Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateApprovalAsync(It.IsAny<Approval>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsSameAsCreatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Approval randomApproval = CreateRandomApproval(randomDateTimeOffset);
            Approval invalidApproval = randomApproval;
            var invalidApprovalException = new InvalidApprovalException();

            invalidApprovalException.AddData(
                key: nameof(Approval.UpdatedDate),
                values: $"Date is the same as {nameof(Approval.CreatedDate)}");

            var expectedApprovalValidationException =
                new ApprovalValidationException(invalidApprovalException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTimeOffset);

            // when
            ValueTask<Approval> modifyApprovalTask =
                this.approvalService.ModifyApprovalAsync(invalidApproval);

            // then
            await Assert.ThrowsAsync<ApprovalValidationException>(() =>
                modifyApprovalTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedApprovalValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectApprovalByIdAsync(invalidApproval.Id),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(MinutesBeforeOrAfter))]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotRecentAndLogItAsync(int minutes)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Approval randomApproval = CreateRandomApproval(randomDateTimeOffset);
            randomApproval.UpdatedDate = randomDateTimeOffset.AddMinutes(minutes);

            var invalidApprovalException =
                new InvalidApprovalException();

            invalidApprovalException.AddData(
                key: nameof(Approval.UpdatedDate),
                values: "Date is not recent");

            var expectedApprovalValidatonException =
                new ApprovalValidationException(invalidApprovalException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                .Returns(randomDateTimeOffset);

            // when
            ValueTask<Approval> modifyApprovalTask =
                this.approvalService.ModifyApprovalAsync(randomApproval);

            // then
            await Assert.ThrowsAsync<ApprovalValidationException>(() =>
                modifyApprovalTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedApprovalValidatonException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectApprovalByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfApprovalDoesNotExistAndLogItAsync()
        {
            // given
            int randomNegativeMinutes = GetRandomNegativeNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Approval randomApproval = CreateRandomApproval(randomDateTimeOffset);
            Approval nonExistApproval = randomApproval;
            nonExistApproval.CreatedDate = randomDateTimeOffset.AddMinutes(randomNegativeMinutes);
            Approval nullApproval = null;

            var notFoundApprovalException =
                new NotFoundApprovalException(nonExistApproval.Id);

            var expectedApprovalValidationException =
                new ApprovalValidationException(notFoundApprovalException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectApprovalByIdAsync(nonExistApproval.Id))
                .ReturnsAsync(nullApproval);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                .Returns(randomDateTimeOffset);

            // when 
            ValueTask<Approval> modifyApprovalTask =
                this.approvalService.ModifyApprovalAsync(nonExistApproval);

            // then
            await Assert.ThrowsAsync<ApprovalValidationException>(() =>
                modifyApprovalTask.AsTask());

            this.storageBrokerMock.Verify(broker =>
                broker.SelectApprovalByIdAsync(nonExistApproval.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedApprovalValidationException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageCreatedDateNotSameAsCreatedDateAndLogItAsync()
        {
            // given
            int randomNumber = GetRandomNumber();
            int randomMinutes = randomNumber;
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Approval randomApproval = CreateRandomModifyApproval(randomDateTimeOffset);
            Approval invalidApproval = randomApproval;
            Approval storageApproval = randomApproval.DeepClone();
            invalidApproval.CreatedDate = storageApproval.CreatedDate.AddMinutes(randomMinutes);
            var invalidApprovalException = new InvalidApprovalException();

            invalidApprovalException.AddData(
                key: nameof(Approval.CreatedDate),
                values: $"Date is not the same as {nameof(Approval.CreatedDate)}");

            var expectedApprovalValidationException =
                new ApprovalValidationException(invalidApprovalException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectApprovalByIdAsync(invalidApproval.Id))
                .ReturnsAsync(storageApproval);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                .Returns(randomDateTimeOffset);

            // when
            ValueTask<Approval> modifyApprovalTask =
                this.approvalService.ModifyApprovalAsync(invalidApproval);

            // then
            await Assert.ThrowsAsync<ApprovalValidationException>(() =>
                modifyApprovalTask.AsTask());

            this.storageBrokerMock.Verify(broker =>
                broker.SelectApprovalByIdAsync(invalidApproval.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogError(It.Is(SameExceptionAs(
                   expectedApprovalValidationException))),
                       Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageCreatedUserIdNotSameAsCreatedUserIdAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Approval randomApproval = CreateRandomModifyApproval(randomDateTimeOffset);
            Approval invalidApproval = randomApproval;
            Approval storageApproval = randomApproval.DeepClone();
            invalidApproval.CreatedByUserId = Guid.NewGuid();
            var invalidApprovalException = new InvalidApprovalException();

            invalidApprovalException.AddData(
                key: nameof(Approval.CreatedByUserId),
                values: $"Id is not the same as {nameof(Approval.CreatedByUserId)}");

            var expectedApprovalValidationException =
                new ApprovalValidationException(invalidApprovalException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectApprovalByIdAsync(invalidApproval.Id))
                .ReturnsAsync(storageApproval);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                .Returns(randomDateTimeOffset);

            // when
            ValueTask<Approval> modifyApprovalTask =
                this.approvalService.ModifyApprovalAsync(invalidApproval);

            // then
            await Assert.ThrowsAsync<ApprovalValidationException>(() =>
                modifyApprovalTask.AsTask());

            this.storageBrokerMock.Verify(broker =>
                broker.SelectApprovalByIdAsync(invalidApproval.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogError(It.Is(SameExceptionAs(
                   expectedApprovalValidationException))),
                       Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
