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
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            var invalidApprovalId = Guid.Empty;

            var invalidApprovalException =
                new InvalidApprovalException();

            invalidApprovalException.AddData(
                key: nameof(Approval.Id),
                values: "Id is required");

            var expectedApprovalValidationException =
                new ApprovalValidationException(invalidApprovalException);

            // when
            ValueTask<Approval> retrieveApprovalByIdTask =
                this.approvalService.RetrieveApprovalByIdAsync(invalidApprovalId);

            // then
            await Assert.ThrowsAsync<ApprovalValidationException>(() =>
                retrieveApprovalByIdTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedApprovalValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectApprovalByIdAsync(It.IsAny<Guid>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRetrieveByIdIfApprovalIsNotFoundAndLogItAsync()
        {
            //given
            Guid someApprovalId = Guid.NewGuid();
            Approval noApproval = null;

            var notFoundApprovalException =
                new NotFoundApprovalException(someApprovalId);

            var expectedApprovalValidationException =
                new ApprovalValidationException(notFoundApprovalException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectApprovalByIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(noApproval);

            //when
            ValueTask<Approval> retrieveApprovalByIdTask =
                this.approvalService.RetrieveApprovalByIdAsync(someApprovalId);

            //then
            await Assert.ThrowsAsync<ApprovalValidationException>(() =>
               retrieveApprovalByIdTask.AsTask());

            this.storageBrokerMock.Verify(broker =>
                broker.SelectApprovalByIdAsync(It.IsAny<Guid>()),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedApprovalValidationException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
