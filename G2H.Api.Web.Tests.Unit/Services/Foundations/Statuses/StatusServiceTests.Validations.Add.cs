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
using G2H.Api.Web.Models.Statuses;
using G2H.Api.Web.Models.Statuses.Exceptions;
using Moq;
using Xunit;

namespace G2H.Api.Web.Tests.Unit.Services.Foundations.Statuses
{
    public partial class StatusServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfStatusIsNullAndLogItAsync()
        {
            // given
            Status nullStatus = null;

            var nullStatusException =
                new NullStatusException();

            var expectedStatusValidationException =
                new StatusValidationException(nullStatusException);

            // when
            ValueTask<Status> addStatusTask =
                this.statusService.AddStatusAsync(nullStatus);

            // then
            await Assert.ThrowsAsync<StatusValidationException>(() =>
                addStatusTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedStatusValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfStatusIsInvalidAndLogItAsync(
            string invalidText)
        {
            // given
            var invalidStatus = new Status
            {
                Name = invalidText
            };

            var invalidStatusException =
                new InvalidStatusException();

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
                values: "Date is required");

            invalidStatusException.AddData(
                key: nameof(Status.UpdatedByUserId),
                values: "Id is required");

            var expectedStatusValidationException =
                new StatusValidationException(invalidStatusException);

            // when
            ValueTask<Status> addStatusTask =
                this.statusService.AddStatusAsync(invalidStatus);

            // then
            await Assert.ThrowsAsync<StatusValidationException>(() =>
               addStatusTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedStatusValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertStatusAsync(It.IsAny<Status>()),
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
            Status randomStatus = CreateRandomStatus();
            Status invalidStatus = randomStatus;

            invalidStatus.UpdatedDate =
                invalidStatus.CreatedDate.AddDays(randomNumber);

            var invalidStatusException =
                new InvalidStatusException();

            invalidStatusException.AddData(
                key: nameof(Status.UpdatedDate),
                values: $"Date is not the same as {nameof(Status.CreatedDate)}");

            var expectedStatusValidationException =
                new StatusValidationException(invalidStatusException);

            // when
            ValueTask<Status> addStatusTask =
                this.statusService.AddStatusAsync(invalidStatus);

            // then
            await Assert.ThrowsAsync<StatusValidationException>(() =>
               addStatusTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedStatusValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertStatusAsync(It.IsAny<Status>()),
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
            Status randomStatus = CreateRandomStatus();
            Status invalidStatus = randomStatus;

            invalidStatus.UpdatedByUserId = Guid.NewGuid();

            var invalidStatusException =
                new InvalidStatusException();

            invalidStatusException.AddData(
                key: nameof(Status.UpdatedByUserId),
                values: $"Id is not the same as {nameof(Status.CreatedByUserId)}");

            var expectedStatusValidationException =
                new StatusValidationException(invalidStatusException);

            // when
            ValueTask<Status> addStatusTask =
                this.statusService.AddStatusAsync(invalidStatus);

            // then
            await Assert.ThrowsAsync<StatusValidationException>(() =>
               addStatusTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedStatusValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertStatusAsync(It.IsAny<Status>()),
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

            Status randomStatus = CreateRandomStatus(invalidDateTime);
            Status invalidStatus = randomStatus;

            var invalidStatusException =
                new InvalidStatusException();

            invalidStatusException.AddData(
                key: nameof(Status.CreatedDate),
                values: "Date is not recent");

            var expectedStatusValidationException =
                new StatusValidationException(invalidStatusException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTimeOffset);

            // when
            ValueTask<Status> addStatusTask =
                this.statusService.AddStatusAsync(invalidStatus);

            // then
            await Assert.ThrowsAsync<StatusValidationException>(() =>
               addStatusTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedStatusValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertStatusAsync(It.IsAny<Status>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
