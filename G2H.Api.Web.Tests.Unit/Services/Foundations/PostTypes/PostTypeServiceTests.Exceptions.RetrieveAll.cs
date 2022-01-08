// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System;
using G2H.Api.Web.Models.PostTypes.Exceptions;
using Microsoft.Data.SqlClient;
using Moq;
using Xunit;

namespace G2H.Api.Web.Tests.Unit.Services.Foundations.PostTypes
{
    public partial class PostTypeServiceTests
    {
        [Fact]
        public void ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSqlExceptionOccursAndLogIt()
        {
            // given
            SqlException sqlException = GetSqlException();

            var failedStorageException =
                new FailedPostTypeStorageException(sqlException);

            var expectedPostTypeDependencyException =
                new PostTypeDependencyException(failedStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllPostTypes())
                    .Throws(sqlException);

            // when
            Action retrieveAllPostTypesAction = () =>
                this.postTypeService.RetrieveAllPostTypes();

            // then
            Assert.Throws<PostTypeDependencyException>(
                retrieveAllPostTypesAction);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllPostTypes(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedPostTypeDependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void ShouldThrowServiceExceptionOnRetrieveAllIfServiceErrorOccursAndLogItAsync()
        {
            // given
            string exceptionMessage = GetRandomMessage();
            var serviceException = new Exception(exceptionMessage);

            var failedPostTypeServiceException =
                new FailedPostTypeServiceException(serviceException);

            var expectedPostTypeServiceException =
                new PostTypeServiceException(failedPostTypeServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllPostTypes())
                    .Throws(serviceException);

            // when
            Action retrieveAllPostTypesAction = () =>
                this.postTypeService.RetrieveAllPostTypes();

            // then
            Assert.Throws<PostTypeServiceException>(
                retrieveAllPostTypesAction);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllPostTypes(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPostTypeServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
