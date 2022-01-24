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
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Approval someApproval = CreateRandomApproval();
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
                this.approvalService.AddApprovalAsync(someApproval);

            // then
            await Assert.ThrowsAsync<ApprovalDependencyException>(() =>
               addApprovalTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertApprovalAsync(It.IsAny<Approval>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedApprovalDependencyException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfApprovalAlreadyExsitsAndLogItAsync()
        {
            // given
            Approval randomApproval = CreateRandomApproval();
            Approval alreadyExistsApproval = randomApproval;
            string randomMessage = GetRandomMessage();

            var duplicateKeyException =
                new DuplicateKeyException(randomMessage);

            var alreadyExistsApprovalException =
                new AlreadyExistsApprovalException(duplicateKeyException);

            var expectedApprovalDependencyValidationException =
                new ApprovalDependencyValidationException(alreadyExistsApprovalException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(duplicateKeyException);

            // when
            ValueTask<Approval> addApprovalTask =
                this.approvalService.AddApprovalAsync(alreadyExistsApproval);

            // then
            await Assert.ThrowsAsync<ApprovalDependencyValidationException>(() =>
                addApprovalTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertApprovalAsync(It.IsAny<Approval>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedApprovalDependencyValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async void ShouldThrowValidationExceptionOnAddIfReferenceErrorOccursAndLogItAsync()
        {
            // given
            Approval someApproval = CreateRandomApproval();
            string randomMessage = GetRandomMessage();
            string exceptionMessage = randomMessage;

            var foreignKeyConstraintConflictException =
                new ForeignKeyConstraintConflictException(exceptionMessage);

            var invalidApprovalReferenceException =
                new InvalidApprovalReferenceException(foreignKeyConstraintConflictException);

            var expectedApprovalValidationException =
                new ApprovalDependencyValidationException(invalidApprovalReferenceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(foreignKeyConstraintConflictException);

            // when
            ValueTask<Approval> addApprovalTask =
                this.approvalService.AddApprovalAsync(someApproval);

            // then
            await Assert.ThrowsAsync<ApprovalDependencyValidationException>(() =>
                addApprovalTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedApprovalValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertApprovalAsync(someApproval),
                    Times.Never());

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDatabaseUpdateErrorOccursAndLogItAsync()
        {
            // given
            Approval someApproval = CreateRandomApproval();

            var databaseUpdateException =
                new DbUpdateException();

            var failedApprovalStorageException =
                new FailedApprovalStorageException(databaseUpdateException);

            var expectedApprovalDependencyException =
                new ApprovalDependencyException(failedApprovalStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(databaseUpdateException);

            // when
            ValueTask<Approval> addApprovalTask =
                this.approvalService.AddApprovalAsync(someApproval);

            // then
            await Assert.ThrowsAsync<ApprovalDependencyException>(() =>
               addApprovalTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertApprovalAsync(It.IsAny<Approval>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedApprovalDependencyException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfServiceErrorOccursAndLogItAsync()
        {
            // given
            Approval someApproval = CreateRandomApproval();
            var serviceException = new Exception();

            var failedApprovalServiceException =
                new FailedApprovalServiceException(serviceException);

            var expectedApprovalServiceException =
                new ApprovalServiceException(failedApprovalServiceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(serviceException);

            // when
            ValueTask<Approval> addApprovalTask =
                this.approvalService.AddApprovalAsync(someApproval);

            // then
            await Assert.ThrowsAsync<ApprovalServiceException>(() =>
                addApprovalTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertApprovalAsync(It.IsAny<Approval>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedApprovalServiceException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
