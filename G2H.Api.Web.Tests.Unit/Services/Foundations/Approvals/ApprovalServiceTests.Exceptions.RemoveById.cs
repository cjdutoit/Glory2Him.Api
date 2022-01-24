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
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace G2H.Api.Web.Tests.Unit.Services.Foundations.Approvals
{
    public partial class ApprovalServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRemoveIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Approval randomApproval = CreateRandomApproval();
            SqlException sqlException = GetSqlException();

            var failedApprovalStorageException =
                new FailedApprovalStorageException(sqlException);

            var expectedApprovalDependencyException =
                new ApprovalDependencyException(failedApprovalStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectApprovalByIdAsync(randomApproval.Id))
                    .Throws(sqlException);

            // when
            ValueTask<Approval> addApprovalTask =
                this.approvalService.RemoveApprovalByIdAsync(randomApproval.Id);

            // then
            await Assert.ThrowsAsync<ApprovalDependencyException>(() =>
               addApprovalTask.AsTask());


            this.storageBrokerMock.Verify(broker =>
                broker.SelectApprovalByIdAsync(randomApproval.Id),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedApprovalDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteApprovalAsync(It.IsAny<Approval>()),
                    Times.Never);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationOnRemoveIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Guid someApprovalId = Guid.NewGuid();

            var databaseUpdateConcurrencyException =
                new DbUpdateConcurrencyException();

            var lockedApprovalException =
                new LockedApprovalException(databaseUpdateConcurrencyException);

            var expectedApprovalDependencyValidationException =
                new ApprovalDependencyValidationException(lockedApprovalException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectApprovalByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(databaseUpdateConcurrencyException);

            // when
            ValueTask<Approval> removeApprovalByIdTask =
                this.approvalService.RemoveApprovalByIdAsync(someApprovalId);

            // then
            await Assert.ThrowsAsync<ApprovalDependencyValidationException>(() =>
                removeApprovalByIdTask.AsTask());

            this.storageBrokerMock.Verify(broker =>
                broker.SelectApprovalByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedApprovalDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteApprovalAsync(It.IsAny<Approval>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRemoveWhenSqlExceptionOccursAndLogItAsync()
        {
            // given
            Guid someApprovalId = Guid.NewGuid();
            SqlException sqlException = GetSqlException();

            var failedApprovalStorageException =
                new FailedApprovalStorageException(sqlException);

            var expectedApprovalDependencyException =
                new ApprovalDependencyException(failedApprovalStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectApprovalByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Approval> deleteApprovalTask =
                this.approvalService.RemoveApprovalByIdAsync(someApprovalId);

            // then
            await Assert.ThrowsAsync<ApprovalDependencyException>(() =>
                deleteApprovalTask.AsTask());

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
        public async Task ShouldThrowServiceExceptionOnRemoveIfExceptionOccursAndLogItAsync()
        {
            // given
            Guid someApprovalId = Guid.NewGuid();
            var serviceException = new Exception();

            var failedApprovalServiceException =
                new FailedApprovalServiceException(serviceException);

            var expectedApprovalServiceException =
                new ApprovalServiceException(failedApprovalServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectApprovalByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<Approval> removeApprovalByIdTask =
                this.approvalService.RemoveApprovalByIdAsync(someApprovalId);

            // then
            await Assert.ThrowsAsync<ApprovalServiceException>(() =>
                removeApprovalByIdTask.AsTask());

            this.storageBrokerMock.Verify(broker =>
                broker.SelectApprovalByIdAsync(It.IsAny<Guid>()),
                        Times.Once());

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
