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
using Microsoft.Data.SqlClient;
using Moq;
using Xunit;

namespace G2H.Api.Web.Tests.Unit.Services.Foundations.Statuses
{
    public partial class StatusServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            StatusId someId = GetRandomStatusId();
            SqlException sqlException = GetSqlException();

            var failedStatusStorageException =
                new FailedStatusStorageException(sqlException);

            var expectedStatusDependencyException =
                new StatusDependencyException(failedStatusStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectStatusByIdAsync(It.IsAny<StatusId>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Status> retrieveStatusByIdTask =
                this.statusService.RetrieveStatusByIdAsync(someId);

            // then
            await Assert.ThrowsAsync<StatusDependencyException>(() =>
                retrieveStatusByIdTask.AsTask());

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
    }
}
