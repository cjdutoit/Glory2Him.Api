﻿// --------------------------------------------------------------------------------
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
using G2H.Api.Web.Models.Statuses;
using G2H.Api.Web.Models.Statuses.Exceptions;
using Moq;
using Xunit;

namespace G2H.Api.Web.Tests.Unit.Services.Foundations.Statuses
{
    public partial class StatusServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStatusIsNullAndLogItAsync()
        {
            // given
            Status nullStatus = null;
            var nullStatusException = new NullStatusException();

            var expectedStatusValidationException =
                new StatusValidationException(nullStatusException);

            // when
            ValueTask<Status> modifyStatusTask =
                this.statusService.ModifyStatusAsync(nullStatus);

            // then
            await Assert.ThrowsAsync<StatusValidationException>(() =>
                modifyStatusTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedStatusValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateStatusAsync(It.IsAny<Status>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfStatusIsInvalidAndLogItAsync(string invalidText)
        {
            // given 
            var invalidStatus = new Status
            {
                Name = invalidText,
            };

            var invalidStatusException = new InvalidStatusException();

            invalidStatusException.AddData(
                key: nameof(Status.Id),
                values: "Id is required");

            invalidStatusException.AddData(
                key: nameof(Status.Name),
                values: "Text is required");

            invalidStatusException.AddData(
                key: nameof(Status.CreatedDate),
                values: "Date is required");

            invalidStatusException.AddData(
                key: nameof(Status.CreatedByUserId),
                values: "Id is required");

            invalidStatusException.AddData(
                key: nameof(Status.UpdatedDate),
                values:
                new[] {
                    "Date is required",
                    $"Date is the same as {nameof(Status.CreatedDate)}"
                });

            invalidStatusException.AddData(
                key: nameof(Status.UpdatedByUserId),
                values: "Id is required");

            var expectedStatusValidationException =
                new StatusValidationException(invalidStatusException);

            // when
            ValueTask<Status> modifyStatusTask =
                this.statusService.ModifyStatusAsync(invalidStatus);

            //then
            await Assert.ThrowsAsync<StatusValidationException>(() =>
                modifyStatusTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedStatusValidationException))),
                        Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateStatusAsync(It.IsAny<Status>()),
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
            Status randomStatus = CreateRandomStatus(randomDateTimeOffset);
            Status invalidStatus = randomStatus;
            var invalidStatusException = new InvalidStatusException();

            invalidStatusException.AddData(
                key: nameof(Status.UpdatedDate),
                values: $"Date is the same as {nameof(Status.CreatedDate)}");

            var expectedStatusValidationException =
                new StatusValidationException(invalidStatusException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTimeOffset);

            // when
            ValueTask<Status> modifyStatusTask =
                this.statusService.ModifyStatusAsync(invalidStatus);

            // then
            await Assert.ThrowsAsync<StatusValidationException>(() =>
                modifyStatusTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedStatusValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectStatusByIdAsync(invalidStatus.Id),
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
            Status randomStatus = CreateRandomStatus(randomDateTimeOffset);
            randomStatus.UpdatedDate = randomDateTimeOffset.AddMinutes(minutes);

            var invalidStatusException =
                new InvalidStatusException();

            invalidStatusException.AddData(
                key: nameof(Status.UpdatedDate),
                values: "Date is not recent");

            var expectedStatusValidatonException =
                new StatusValidationException(invalidStatusException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                .Returns(randomDateTimeOffset);

            // when
            ValueTask<Status> modifyStatusTask =
                this.statusService.ModifyStatusAsync(randomStatus);

            // then
            await Assert.ThrowsAsync<StatusValidationException>(() =>
                modifyStatusTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedStatusValidatonException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectStatusByIdAsync(It.IsAny<StatusId>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStatusDoesNotExistAndLogItAsync()
        {
            // given
            int randomNegativeMinutes = GetRandomNegativeNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Status randomStatus = CreateRandomStatus(randomDateTimeOffset);
            Status nonExistStatus = randomStatus;
            nonExistStatus.CreatedDate = randomDateTimeOffset.AddMinutes(randomNegativeMinutes);
            Status nullStatus = null;

            var notFoundStatusException =
                new NotFoundStatusException(nonExistStatus.Id);

            var expectedStatusValidationException =
                new StatusValidationException(notFoundStatusException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectStatusByIdAsync(nonExistStatus.Id))
                .ReturnsAsync(nullStatus);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                .Returns(randomDateTimeOffset);

            // when 
            ValueTask<Status> modifyStatusTask =
                this.statusService.ModifyStatusAsync(nonExistStatus);

            // then
            await Assert.ThrowsAsync<StatusValidationException>(() =>
                modifyStatusTask.AsTask());

            this.storageBrokerMock.Verify(broker =>
                broker.SelectStatusByIdAsync(nonExistStatus.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedStatusValidationException))),
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
            Status randomStatus = CreateRandomModifyStatus(randomDateTimeOffset);
            Status invalidStatus = randomStatus;
            Status storageStatus = randomStatus.DeepClone();
            invalidStatus.CreatedDate = storageStatus.CreatedDate.AddMinutes(randomMinutes);
            var invalidStatusException = new InvalidStatusException();

            invalidStatusException.AddData(
                key: nameof(Status.CreatedDate),
                values: $"Date is not the same as {nameof(Status.CreatedDate)}");

            var expectedStatusValidationException =
                new StatusValidationException(invalidStatusException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectStatusByIdAsync(invalidStatus.Id))
                .ReturnsAsync(storageStatus);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                .Returns(randomDateTimeOffset);

            // when
            ValueTask<Status> modifyStatusTask =
                this.statusService.ModifyStatusAsync(invalidStatus);

            // then
            await Assert.ThrowsAsync<StatusValidationException>(() =>
                modifyStatusTask.AsTask());

            this.storageBrokerMock.Verify(broker =>
                broker.SelectStatusByIdAsync(invalidStatus.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogError(It.Is(SameExceptionAs(
                   expectedStatusValidationException))),
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
            Status randomStatus = CreateRandomModifyStatus(randomDateTimeOffset);
            Status invalidStatus = randomStatus;
            Status storageStatus = randomStatus.DeepClone();
            invalidStatus.CreatedByUserId = Guid.NewGuid();
            var invalidStatusException = new InvalidStatusException();

            invalidStatusException.AddData(
                key: nameof(Status.CreatedByUserId),
                values: $"Id is not the same as {nameof(Status.CreatedByUserId)}");

            var expectedStatusValidationException =
                new StatusValidationException(invalidStatusException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectStatusByIdAsync(invalidStatus.Id))
                .ReturnsAsync(storageStatus);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                .Returns(randomDateTimeOffset);

            // when
            ValueTask<Status> modifyStatusTask =
                this.statusService.ModifyStatusAsync(invalidStatus);

            // then
            await Assert.ThrowsAsync<StatusValidationException>(() =>
                modifyStatusTask.AsTask());

            this.storageBrokerMock.Verify(broker =>
                broker.SelectStatusByIdAsync(invalidStatus.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogError(It.Is(SameExceptionAs(
                   expectedStatusValidationException))),
                       Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageUpdatedDateSameAsUpdatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Status randomStatus = CreateRandomModifyStatus(randomDateTimeOffset);
            Status invalidStatus = randomStatus;

            Status storageStatus = randomStatus.DeepClone();
            invalidStatus.UpdatedDate = storageStatus.UpdatedDate;

            var invalidStatusException = new InvalidStatusException();

            invalidStatusException.AddData(
                key: nameof(Status.UpdatedDate),
                values: $"Date is the same as {nameof(Status.UpdatedDate)}");

            var expectedStatusValidationException =
                new StatusValidationException(invalidStatusException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectStatusByIdAsync(invalidStatus.Id))
                .ReturnsAsync(storageStatus);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTimeOffset);

            // when
            ValueTask<Status> modifyStatusTask =
                this.statusService.ModifyStatusAsync(invalidStatus);

            // then
            await Assert.ThrowsAsync<StatusValidationException>(() =>
                modifyStatusTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedStatusValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectStatusByIdAsync(invalidStatus.Id),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
