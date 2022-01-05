// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

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
                    "Date is required"
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
    }
}
