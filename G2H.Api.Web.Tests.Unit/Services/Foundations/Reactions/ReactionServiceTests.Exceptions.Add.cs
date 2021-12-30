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
using G2H.Api.Web.Models.Reactions;
using G2H.Api.Web.Models.Reactions.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace G2H.Api.Web.Tests.Unit.Services.Foundations.Reactions
{
    public partial class ReactionServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnAddIfSqlErrorOccursAndLogItAsync()
        {
            // given
            Reaction someReaction = CreateRandomReaction();
            SqlException sqlException = GetSqlException();

            var failedReactionStorageException =
                new FailedReactionStorageException(sqlException);

            var expectedReactionDependencyException =
                new ReactionDependencyException(failedReactionStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(sqlException);

            // when
            ValueTask<Reaction> addReactionTask =
                this.reactionService.AddReactionAsync(someReaction);

            // then
            await Assert.ThrowsAsync<ReactionDependencyException>(() =>
               addReactionTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertReactionAsync(It.IsAny<Reaction>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedReactionDependencyException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyValidationExceptionOnAddIfReactionAlreadyExsitsAndLogItAsync()
        {
            // given
            Reaction randomReaction = CreateRandomReaction();
            Reaction alreadyExistsReaction = randomReaction;
            string randomMessage = GetRandomMessage();

            var duplicateKeyException =
                new DuplicateKeyException(randomMessage);

            var alreadyExistsReactionException =
                new AlreadyExistsReactionException(duplicateKeyException);

            var expectedReactionDependencyValidationException =
                new ReactionDependencyValidationException(alreadyExistsReactionException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(duplicateKeyException);

            // when
            ValueTask<Reaction> addReactionTask =
                this.reactionService.AddReactionAsync(alreadyExistsReaction);

            // then
            await Assert.ThrowsAsync<ReactionDependencyValidationException>(() =>
                addReactionTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertReactionAsync(It.IsAny<Reaction>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedReactionDependencyValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowDependencyExceptionOnAddIfDatabaseUpdateErrorOccursAndLogItAsync()
        {
            // given
            Reaction someReaction = CreateRandomReaction();

            var databaseUpdateException =
                new DbUpdateException();

            var failedReactionStorageException =
                new FailedReactionStorageException(databaseUpdateException);

            var expectedReactionDependencyException =
                new ReactionDependencyException(failedReactionStorageException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(databaseUpdateException);

            // when
            ValueTask<Reaction> addReactionTask =
                this.reactionService.AddReactionAsync(someReaction);

            // then
            await Assert.ThrowsAsync<ReactionDependencyException>(() =>
               addReactionTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertReactionAsync(It.IsAny<Reaction>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedReactionDependencyException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowServiceExceptionOnAddIfDatabaseUpdateErrorOccursAndLogItAsync()
        {
            // given
            Reaction someReaction = CreateRandomReaction();
            var serviceException = new Exception();

            var failedReactionServiceException =
                new FailedReactionServiceException(serviceException);

            var expectedReactionServiceException =
                new ReactionServiceException(failedReactionServiceException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Throws(serviceException);

            // when
            ValueTask<Reaction> addReactionTask =
                this.reactionService.AddReactionAsync(someReaction);

            // then
            await Assert.ThrowsAsync<ReactionServiceException>(() =>
                addReactionTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertReactionAsync(It.IsAny<Reaction>()),
                    Times.Never);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedReactionServiceException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
