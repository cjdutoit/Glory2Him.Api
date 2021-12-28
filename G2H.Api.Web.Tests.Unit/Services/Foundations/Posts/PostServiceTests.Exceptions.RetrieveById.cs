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
using G2H.Api.Web.Models.Posts;
using G2H.Api.Web.Models.Posts.Exceptions;
using Microsoft.Data.SqlClient;
using Moq;
using Xunit;

namespace G2H.Api.Web.Tests.Unit.Services.Foundations.Posts
{
    public partial class PostServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Guid someId = Guid.NewGuid();
            SqlException sqlException = GetSqlException();

            var failedPostStorageException =
                new FailedPostStorageException(sqlException);

            var expectedPostDependencyException =
                new PostDependencyException(failedPostStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPostByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Post> retrievePostByIdTask =
                this.postService.RetrievePostByIdAsync(someId);

            // then
            await Assert.ThrowsAsync<PostDependencyException>(() =>
                retrievePostByIdTask.AsTask());

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPostByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedPostDependencyException))),
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

            var failedPostServiceException =
                new FailedPostServiceException(serviceException);

            var expectedPostServiceException =
                new PostServiceException(failedPostServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPostByIdAsync(It.IsAny<Guid>()))
                    .ThrowsAsync(serviceException);

            // when
            ValueTask<Post> retrievePostByIdTask =
                this.postService.RetrievePostByIdAsync(someId);

            // then
            await Assert.ThrowsAsync<PostServiceException>(() =>
                retrievePostByIdTask.AsTask());

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPostByIdAsync(It.IsAny<Guid>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogError(It.Is(SameExceptionAs(
                   expectedPostServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
