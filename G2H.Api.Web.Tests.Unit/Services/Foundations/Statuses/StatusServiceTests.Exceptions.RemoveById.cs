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
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace G2H.Api.Web.Tests.Unit.Services.Foundations.Statuses
{
    public partial class StatusServiceTests
    {
        [Fact]
        public async Task ShouldThrowDependencyValidationOnRemoveIfDatabaseUpdateConcurrencyErrorOccursAndLogItAsync()
        {
            // given
            StatusId someStatusId = GetRandomStatusId();

            var databaseUpdateConcurrencyException =
                new DbUpdateConcurrencyException();

            var lockedStatusException =
                new LockedStatusException(databaseUpdateConcurrencyException);

            var expectedStatusDependencyValidationException =
                new StatusDependencyValidationException(lockedStatusException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectStatusByIdAsync(It.IsAny<StatusId>()))
                    .ThrowsAsync(databaseUpdateConcurrencyException);

            // when
            ValueTask<Status> removeStatusByIdTask =
                this.statusService.RemoveStatusByIdAsync(someStatusId);

            // then
            await Assert.ThrowsAsync<StatusDependencyValidationException>(() =>
                removeStatusByIdTask.AsTask());

            this.storageBrokerMock.Verify(broker =>
                broker.SelectStatusByIdAsync(It.IsAny<StatusId>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedStatusDependencyValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteStatusAsync(It.IsAny<Status>()),
                    Times.Never);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnRemoveWhenSqlExceptionOccursAndLogItAsync()
        {
            // given
            StatusId someStatusId = GetRandomStatusId();
            SqlException sqlException = GetSqlException();

            var failedStatusStorageException =
                new FailedStatusStorageException(sqlException);

            var expectedStatusDependencyException =
                new StatusDependencyException(failedStatusStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectStatusByIdAsync(It.IsAny<StatusId>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Status> deleteStatusTask =
                this.statusService.RemoveStatusByIdAsync(someStatusId);

            // then
            await Assert.ThrowsAsync<StatusDependencyException>(() =>
                deleteStatusTask.AsTask());

            this.storageBrokerMock.Verify(broker =>
                broker.SelectStatusByIdAsync(It.IsAny<StatusId>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedStatusDependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnRemoveIfExceptionOccursAndLogItAsync()
        {
            // given
            StatusId someStatusId = GetRandomStatusId();
            var serviceException = new Exception();

            var failedStatusServiceException =
                new FailedStatusServiceException(serviceException);

            var expectedStatusServiceException =
                new StatusServiceException(failedStatusServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectStatusByIdAsync(It.IsAny<StatusId>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<Status> removeStatusByIdTask =
                this.statusService.RemoveStatusByIdAsync(someStatusId);

            // then
            await Assert.ThrowsAsync<StatusServiceException>(() =>
                removeStatusByIdTask.AsTask());

            this.storageBrokerMock.Verify(broker =>
                broker.SelectStatusByIdAsync(It.IsAny<StatusId>()),
                        Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedStatusServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
