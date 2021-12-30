// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System.Threading.Tasks;
using G2H.Api.Web.Models.Reactions;
using G2H.Api.Web.Models.Reactions.Exceptions;
using Microsoft.Data.SqlClient;
using Moq;
using Xunit;

namespace G2H.Api.Web.Tests.Unit.Services.Foundations.Reactions
{
    public partial class ReactionServiceTests
    {
        [Fact]
        public async Task ShouldThrowCriticalDependencyExceptionOnRetrieveByIdIfSqlErrorOccursAndLogItAsync()
        {
            // given
            ReactionId someId = GetRandomReactionId();
            SqlException sqlException = GetSqlException();

            var failedReactionStorageException =
                new FailedReactionStorageException(sqlException);

            var expectedReactionDependencyException =
                new ReactionDependencyException(failedReactionStorageException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectReactionByIdAsync(It.IsAny<ReactionId>()))
                    .ThrowsAsync(sqlException);

            // when
            ValueTask<Reaction> retrieveReactionByIdTask =
                this.reactionService.RetrieveReactionByIdAsync(someId);

            // then
            await Assert.ThrowsAsync<ReactionDependencyException>(() =>
                retrieveReactionByIdTask.AsTask());

            this.storageBrokerMock.Verify(broker =>
                broker.SelectReactionByIdAsync(It.IsAny<ReactionId>()),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogCritical(It.Is(SameExceptionAs(
                    expectedReactionDependencyException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
