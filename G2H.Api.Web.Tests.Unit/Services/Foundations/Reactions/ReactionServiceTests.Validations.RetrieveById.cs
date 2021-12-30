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
using Moq;
using Xunit;

namespace G2H.Api.Web.Tests.Unit.Services.Foundations.Reactions
{
    public partial class ReactionServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            var invalidReactionId = (ReactionId)default;

            var invalidReactionException =
                new InvalidReactionException();

            invalidReactionException.AddData(
                key: nameof(Reaction.Id),
                values: "Id is required");

            var expectedReactionValidationException =
                new ReactionValidationException(invalidReactionException);

            // when
            ValueTask<Reaction> retrieveReactionByIdTask =
                this.reactionService.RetrieveReactionByIdAsync(invalidReactionId);

            // then
            await Assert.ThrowsAsync<ReactionValidationException>(() =>
                retrieveReactionByIdTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedReactionValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectReactionByIdAsync(It.IsAny<ReactionId>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRetrieveByIdIfReactionIsNotFoundAndLogItAsync()
        {
            //given
            ReactionId someReactionId = GetRandomReactionId();
            Reaction noReaction = null;

            var notFoundReactionException =
                new NotFoundReactionException(someReactionId);

            var expectedReactionValidationException =
                new ReactionValidationException(notFoundReactionException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectReactionByIdAsync(It.IsAny<ReactionId>()))
                    .ReturnsAsync(noReaction);

            //when
            ValueTask<Reaction> retrieveReactionByIdTask =
                this.reactionService.RetrieveReactionByIdAsync(someReactionId);

            //then
            await Assert.ThrowsAsync<ReactionValidationException>(() =>
               retrieveReactionByIdTask.AsTask());

            this.storageBrokerMock.Verify(broker =>
                broker.SelectReactionByIdAsync(It.IsAny<ReactionId>()),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedReactionValidationException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
