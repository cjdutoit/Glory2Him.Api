﻿// --------------------------------------------------------------------------------
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
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            var invalidStatusId = (StatusId)default;

            var invalidStatusException =
                new InvalidStatusException();

            invalidStatusException.AddData(
                key: nameof(Status.Id),
                values: "Id is required");

            var expectedStatusValidationException =
                new StatusValidationException(invalidStatusException);

            // when
            ValueTask<Status> retrieveStatusByIdTask =
                this.statusService.RetrieveStatusByIdAsync(invalidStatusId);

            // then
            await Assert.ThrowsAsync<StatusValidationException>(() =>
                retrieveStatusByIdTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedStatusValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectStatusByIdAsync(It.IsAny<StatusId>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRetrieveByIdIfStatusIsNotFoundAndLogItAsync()
        {
            //given
            StatusId someStatusId = GetRandomStatusId();
            Status noStatus = null;

            var notFoundStatusException =
                new NotFoundStatusException(someStatusId);

            var expectedStatusValidationException =
                new StatusValidationException(notFoundStatusException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectStatusByIdAsync(It.IsAny<StatusId>()))
                    .ReturnsAsync(noStatus);

            //when
            ValueTask<Status> retrieveStatusByIdTask =
                this.statusService.RetrieveStatusByIdAsync(someStatusId);

            //then
            await Assert.ThrowsAsync<StatusValidationException>(() =>
               retrieveStatusByIdTask.AsTask());

            this.storageBrokerMock.Verify(broker =>
                broker.SelectStatusByIdAsync(It.IsAny<StatusId>()),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedStatusValidationException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
