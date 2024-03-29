﻿// --------------------------------------------------------------------------------
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
        public async Task ShouldThrowValidationExceptionOnAddIfReactionIsNullAndLogItAsync()
        {
            // given
            Reaction nullReaction = null;

            var nullReactionException =
                new NullReactionException();

            var expectedReactionValidationException =
                new ReactionValidationException(nullReactionException);

            // when
            ValueTask<Reaction> addReactionTask =
                this.reactionService.AddReactionAsync(nullReaction);

            // then
            await Assert.ThrowsAsync<ReactionValidationException>(() =>
                addReactionTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedReactionValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfReactionIsInvalidAndLogItAsync(
            string invalidText)
        {
            // given
            var invalidReaction = new Reaction
            {
                Name = invalidText
            };

            var invalidReactionException =
                new InvalidReactionException();

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
            ValueTask<Reaction> addReactionTask =
                this.reactionService.AddReactionAsync(invalidReaction);

            // then
            await Assert.ThrowsAsync<ReactionValidationException>(() =>
               addReactionTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedReactionValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertReactionAsync(It.IsAny<Reaction>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCreateAndUpdateDatesIsNotSameAndLogItAsync()
        {
            // given
            Reaction nullReaction = null;

            var nullReactionException =
                new NullReactionException();

            var expectedReactionValidationException =
                new ReactionValidationException(nullReactionException);

            // when
            ValueTask<Reaction> addReactionTask =
                this.reactionService.AddReactionAsync(nullReaction);

            // then
            await Assert.ThrowsAsync<ReactionValidationException>(() =>
                addReactionTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedReactionValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCreateAndUpdateUserIdsIsNotSameAndLogItAsync()
        {
            // given
            int randomNumber = GetRandomNumber();
            Reaction randomReaction = CreateRandomReaction();
            Reaction invalidReaction = randomReaction;

            invalidReaction.UpdatedByUserId = Guid.NewGuid();

            var invalidReactionException =
                new InvalidReactionException();

            invalidReactionException.AddData(
                key: nameof(Reaction.UpdatedByUserId),
                values: $"Id is not the same as {nameof(Reaction.CreatedByUserId)}");

            var expectedReactionValidationException =
                new ReactionValidationException(invalidReactionException);

            // when
            ValueTask<Reaction> addReactionTask =
                this.reactionService.AddReactionAsync(invalidReaction);

            // then
            await Assert.ThrowsAsync<ReactionValidationException>(() =>
               addReactionTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedReactionValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertReactionAsync(It.IsAny<Reaction>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(MinutesBeforeOrAfter))]
        public async Task ShouldThrowValidationExceptionOnAddIfCreatedDateIsNotRecentAndLogItAsync(
            int minutesBeforeOrAfter)
        {
            // given
            DateTimeOffset randomDateTimeOffset =
                GetRandomDateTimeOffset();

            DateTimeOffset invalidDateTime =
                randomDateTimeOffset.AddMinutes(minutesBeforeOrAfter);

            Reaction randomReaction = CreateRandomReaction(invalidDateTime);
            Reaction invalidReaction = randomReaction;

            var invalidReactionException =
                new InvalidReactionException();

            invalidReactionException.AddData(
                key: nameof(Reaction.CreatedDate),
                values: "Date is not recent");

            var expectedReactionValidationException =
                new ReactionValidationException(invalidReactionException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTimeOffset);

            // when
            ValueTask<Reaction> addReactionTask =
                this.reactionService.AddReactionAsync(invalidReaction);

            // then
            await Assert.ThrowsAsync<ReactionValidationException>(() =>
               addReactionTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedReactionValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertReactionAsync(It.IsAny<Reaction>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
