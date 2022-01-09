// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System.Threading.Tasks;
using G2H.Api.Web.Models.PostTypes;
using G2H.Api.Web.Models.PostTypes.Exceptions;
using Microsoft.Data.SqlClient;
using Moq;
using Xunit;

namespace G2H.Api.Web.Tests.Unit.Services.Foundations.PostTypes
{
    public partial class PostTypeServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            PostTypeId someId = GetRandomPostTypeId();
            SqlException sqlException = GetSqlException();

            var failedPostTypeStorageException =
                new FailedPostTypeStorageException(sqlException);

            var expectedPostTypeDependencyException =
                new PostTypeDependencyException(failedPostTypeStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPostTypeByIdAsync(It.IsAny<PostTypeId>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<PostType> retrievePostTypeByIdTask =
                this.postTypeService.RetrievePostTypeByIdAsync(someId);

            // then
            await Assert.ThrowsAsync<PostTypeDependencyException>(() =>
                retrievePostTypeByIdTask.AsTask());

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPostTypeByIdAsync(It.IsAny<PostTypeId>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedPostTypeDependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}