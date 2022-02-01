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
using EFxceptions.Models.Exceptions;
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
        public async Task ShouldThrowCriticalDependencyExceptionOnModifyIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Approval randomApproval = CreateRandomApproval();
            SqlException sqlException = GetSqlException();

            var failedApprovalStorageException =
                new FailedApprovalStorageException(sqlException);

            var expectedApprovalDependencyException =
                new ApprovalDependencyException(failedApprovalStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(sqlException);

            // when
            ValueTask<Approval> addApprovalTask =
                this.approvalService.ModifyApprovalAsync(randomApproval);

            // then
            await Assert.ThrowsAsync<ApprovalDependencyException>(() =>
               addApprovalTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectApprovalByIdAsync(randomApproval.Id),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedApprovalDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateApprovalAsync(randomApproval),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldThrowValidationExceptionOnModifyIfReferenceErrorOccursAndLogItAsync()
        {
            // given
            Approval foreignKeyConflictedApproval = CreateRandomApproval();
            string randomMessage = GetRandomMessage();
            string exceptionMessage = randomMessage;

            var foreignKeyConstraintConflictException =
                new ForeignKeyConstraintConflictException(exceptionMessage);

            var invalidApprovalReferenceException =
                new InvalidApprovalReferenceException(foreignKeyConstraintConflictException);

            var approvalDependencyValidationException =
                new ApprovalDependencyValidationException(invalidApprovalReferenceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(foreignKeyConstraintConflictException);

            // when
            ValueTask<Approval> modifyApprovalTask =
                this.approvalService.ModifyApprovalAsync(foreignKeyConflictedApproval);

            // then
            await Assert.ThrowsAsync<ApprovalDependencyValidationException>(() =>
                modifyApprovalTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectApprovalByIdAsync(foreignKeyConflictedApproval.Id),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(approvalDependencyValidationException))),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateApprovalAsync(foreignKeyConflictedApproval),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnModifyIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            Approval randomApproval = CreateRandomApproval();
            var databaseUpdateConcurrencyException = new DbUpdateConcurrencyException();

            var lockedApprovalException =
                new LockedApprovalException(databaseUpdateConcurrencyException);

            var expectedApprovalDependencyValidationException =
                new ApprovalDependencyValidationException(lockedApprovalException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(databaseUpdateConcurrencyException);

            // when
            ValueTask<Approval> modifyApprovalTask =
                this.approvalService.ModifyApprovalAsync(randomApproval);

            // then
            await Assert.ThrowsAsync<ApprovalDependencyValidationException>(() =>
                modifyApprovalTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectApprovalByIdAsync(randomApproval.Id),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedApprovalDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateApprovalAsync(randomApproval),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnModifyIfDatabaseUpdateExceptionOccursAndLogItAsync()
        {
            // given
            Approval randomApproval = CreateRandomApproval();
            var databaseUpdateException = new DbUpdateException();

            var failedApprovalException =
                new FailedApprovalStorageException(databaseUpdateException);

            var expectedApprovalDependencyException =
                new ApprovalDependencyException(failedApprovalException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(databaseUpdateException);

            // when
            ValueTask<Approval> modifyApprovalTask =
                this.approvalService.ModifyApprovalAsync(randomApproval);

            // then
            await Assert.ThrowsAsync<ApprovalDependencyException>(() =>
                modifyApprovalTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectApprovalByIdAsync(randomApproval.Id),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedApprovalDependencyException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateApprovalAsync(randomApproval),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnModifyIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Approval randomApproval = CreateRandomApproval();
            var serviceException = new Exception();

            var failedApprovalException =
                new FailedApprovalServiceException(serviceException);

            var expectedApprovalServiceException =
                new ApprovalServiceException(failedApprovalException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(serviceException);

            // when
            ValueTask<Approval> modifyApprovalTask =
                this.approvalService.ModifyApprovalAsync(randomApproval);

            // then
            await Assert.ThrowsAsync<ApprovalServiceException>(() =>
                modifyApprovalTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectApprovalByIdAsync(randomApproval.Id),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedApprovalServiceException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateApprovalAsync(randomApproval),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
