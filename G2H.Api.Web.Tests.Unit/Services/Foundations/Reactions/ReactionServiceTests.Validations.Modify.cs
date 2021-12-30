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
using G2H.Api.Web.Models.Reactions;
using G2H.Api.Web.Models.Reactions.Exceptions;
using Moq;
using Xunit;

namespace G2H.Api.Web.Tests.Unit.Services.Foundations.Reactions
{
    public partial class ReactionServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfReactionIsNullAndLogItAsync()
        {
            // given
            Reaction nullReaction = null;
            var nullReactionException = new NullReactionException();

            var expectedReactionValidationException =
                new ReactionValidationException(nullReactionException);

            // when
            ValueTask<Reaction> modifyReactionTask =
                this.reactionService.ModifyReactionAsync(nullReaction);

            // then
            await Assert.ThrowsAsync<ReactionValidationException>(() =>
                modifyReactionTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedReactionValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateReactionAsync(It.IsAny<Reaction>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfReactionIsInvalidAndLogItAsync(string invalidText)
        {
            // given 
            var invalidReaction = new Reaction
            {
                Name = invalidText,
            };

            var invalidReactionException = new InvalidReactionException();

            invalidReactionException.AddData(
                key: nameof(Reaction.Id),
                values: "Id is required");

            invalidReactionException.AddData(
                key: nameof(Reaction.Name),
                values: "Text is required");

            invalidReactionException.AddData(
                key: nameof(Reaction.CreatedDate),
                values: "Date is required");

            invalidReactionException.AddData(
                key: nameof(Reaction.CreatedByUserId),
                values: "Id is required");

            invalidReactionException.AddData(
                key: nameof(Reaction.UpdatedDate),
                values: "Date is required");

            invalidReactionException.AddData(
                key: nameof(Reaction.UpdatedByUserId),
                values: "Id is required");

            var expectedReactionValidationException =
                new ReactionValidationException(invalidReactionException);

            // when
            ValueTask<Reaction> modifyReactionTask =
                this.reactionService.ModifyReactionAsync(invalidReaction);

            //then
            await Assert.ThrowsAsync<ReactionValidationException>(() =>
                modifyReactionTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedReactionValidationException))),
                        Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateReactionAsync(It.IsAny<Reaction>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsSameAsCreatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Reaction randomReaction = CreateRandomReaction(randomDateTimeOffset);
            Reaction invalidReaction = randomReaction;
            var invalidReactionException = new InvalidReactionException();

            invalidReactionException.AddData(
                key: nameof(Reaction.UpdatedDate),
                values: $"Date is the same as {nameof(Reaction.CreatedDate)}");

            var expectedReactionValidationException =
                new ReactionValidationException(invalidReactionException);

            // when
            ValueTask<Reaction> modifyReactionTask =
                this.reactionService.ModifyReactionAsync(invalidReaction);

            // then
            await Assert.ThrowsAsync<ReactionValidationException>(() =>
                modifyReactionTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedReactionValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectReactionByIdAsync(invalidReaction.Id),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(InvalidMinuteCases))]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotRecentAndLogItAsync(int minutes)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Reaction randomReaction = CreateRandomReaction(randomDateTimeOffset);
            randomReaction.UpdatedDate = randomDateTimeOffset.AddMinutes(minutes);

            var invalidReactionException =
                new InvalidReactionException();

            invalidReactionException.AddData(
                key: nameof(Reaction.UpdatedDate),
                values: "Date is not recent");

            var expectedReactionValidatonException =
                new ReactionValidationException(invalidReactionException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                .Returns(randomDateTimeOffset);

            // when
            ValueTask<Reaction> modifyReactionTask =
                this.reactionService.ModifyReactionAsync(randomReaction);

            // then
            await Assert.ThrowsAsync<ReactionValidationException>(() =>
                modifyReactionTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedReactionValidatonException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectReactionByIdAsync(It.IsAny<ReactionId>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
