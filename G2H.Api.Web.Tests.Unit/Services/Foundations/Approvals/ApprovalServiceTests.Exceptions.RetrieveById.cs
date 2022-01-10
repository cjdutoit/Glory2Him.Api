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
using Microsoft.Data.SqlClient;
using Moq;
using Xunit;

namespace G2H.Api.Web.Tests.Unit.Services.Foundations.Approvals
{
    public partial class ApprovalServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            SqlException sqlException = GetSqlException();

            var failedApprovalStorageException =
                new FailedApprovalStorageException(sqlException);

            var expectedApprovalDependencyException =
                new ApprovalDependencyException(failedApprovalStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectApprovalByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Approval> retrieveApprovalByIdTask =
                this.approvalService.RetrieveApprovalByIdAsync(someId);

            // then
            await Assert.ThrowsAsync<ApprovalDependencyException>(() =>
                retrieveApprovalByIdTask.AsTask());

            this.storageBrokerMock.Verify(broker =>
                broker.SelectApprovalByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedApprovalDependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRetrieveByIdIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedApprovalServiceException =
                new FailedApprovalServiceException(serviceException);

            var expectedApprovalServiceException =
                new ApprovalServiceException(failedApprovalServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectApprovalByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<Approval> retrieveApprovalByIdTask =
                this.approvalService.RetrieveApprovalByIdAsync(someId);

            // then
            await Assert.ThrowsAsync<ApprovalServiceException>(() =>
                retrieveApprovalByIdTask.AsTask());

            this.storageBrokerMock.Verify(broker =>
                broker.SelectApprovalByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogError(It.Is(SameExceptionAs(
                   expectedApprovalServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
