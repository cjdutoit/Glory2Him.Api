// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using G2H.Api.Web.Models.PostTypes;
using G2H.Api.Web.Models.PostTypes.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace G2H.Api.Web.Tests.Unit.Services.Foundations.PostTypes
{
    public partial class PostTypeServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlErrorOccursAndLogItAsync()
        {
            // given
            PostType somePostType = CreateRandomPostType();
            SqlException sqlException = GetSqlException();

            var failedPostTypeStorageException =
                new FailedPostTypeStorageException(sqlException);

            var expectedPostTypeDependencyException =
                new PostTypeDependencyException(failedPostTypeStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(sqlException);

            // when
            ValueTask<PostType> addPostTypeTask =
                this.postTypeService.AddPostTypeAsync(somePostType);

            // then
            await Assert.ThrowsAsync<PostTypeDependencyException>(() =>
               addPostTypeTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertPostTypeAsync(It.IsAny<PostType>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedPostTypeDependencyException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfPostTypeAlreadyExsitsAndLogItAsync()
        {
            // given
            PostType randomPostType = CreateRandomPostType();
            PostType alreadyExistsPostType = randomPostType;
            string randomMessage = GetRandomMessage();

            var duplicateKeyException =
                new DuplicateKeyException(randomMessage);

            var alreadyExistsPostTypeException =
                new AlreadyExistsPostTypeException(duplicateKeyException);

            var expectedPostTypeDependencyValidationException =
                new PostTypeDependencyValidationException(alreadyExistsPostTypeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(duplicateKeyException);

            // when
            ValueTask<PostType> addPostTypeTask =
                this.postTypeService.AddPostTypeAsync(alreadyExistsPostType);

            // then
            await Assert.ThrowsAsync<PostTypeDependencyValidationException>(() =>
                addPostTypeTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertPostTypeAsync(It.IsAny<PostType>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPostTypeDependencyValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDatabaseUpdateErrorOccursAndLogItAsync()
        {
            // given
            PostType somePostType = CreateRandomPostType();

            var databaseUpdateException =
                new DbUpdateException();

            var failedPostTypeStorageException =
                new FailedPostTypeStorageException(databaseUpdateException);

            var expectedPostTypeDependencyException =
                new PostTypeDependencyException(failedPostTypeStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(databaseUpdateException);

            // when
            ValueTask<PostType> addPostTypeTask =
                this.postTypeService.AddPostTypeAsync(somePostType);

            // then
            await Assert.ThrowsAsync<PostTypeDependencyException>(() =>
               addPostTypeTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertPostTypeAsync(It.IsAny<PostType>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPostTypeDependencyException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
