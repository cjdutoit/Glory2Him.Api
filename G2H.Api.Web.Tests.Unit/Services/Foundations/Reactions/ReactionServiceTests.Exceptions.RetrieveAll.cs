// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System;
using G2H.Api.Web.Models.Reactions.Exceptions;
using Microsoft.Data.SqlClient;
using Moq;
using Xunit;

namespace G2H.Api.Web.Tests.Unit.Services.Foundations.Reactions
{
    public partial class ReactionServiceTests
    {
        [Fact]
        public void ShouldThrowCriticalDependencyExceptionOnRetrieveAllWhenSqlExceptionOccursAndLogIt()
        {
            // given
            SqlException sqlException = GetSqlException();

            var failedStorageException =
                new FailedReactionStorageException(sqlException);

            var expectedReactionDependencyException =
                new ReactionDependencyException(failedStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllReactions())
                    .Throws(sqlException);

            // when
            Action retrieveAllReactionsAction = () =>
                this.reactionService.RetrieveAllReactions();

            // then
            Assert.Throws<ReactionDependencyException>(
                retrieveAllReactionsAction);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllReactions(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedReactionDependencyException))),
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

            var failedReactionServiceException =
                new FailedReactionServiceException(serviceException);

            var expectedReactionServiceException =
                new ReactionServiceException(failedReactionServiceException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllReactions())
                    .Throws(serviceException);

            // when
            Action retrieveAllReactionsAction = () =>
                this.reactionService.RetrieveAllReactions();

            // then
            Assert.Throws<ReactionServiceException>(
                retrieveAllReactionsAction);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllReactions(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedReactionServiceException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
