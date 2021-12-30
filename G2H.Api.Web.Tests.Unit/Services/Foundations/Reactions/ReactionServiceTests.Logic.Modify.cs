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
using FluentAssertions;
using Force.DeepCloner;
using G2H.Api.Web.Models.Reactions;
using Moq;
using Xunit;

namespace G2H.Api.Web.Tests.Unit.Services.Foundations.Reactions
{
    public partial class ReactionServiceTests
    {
        [Fact]
        public async Task ShouldModifyReactionAsync()
        {
            // given
            int minuteInPast = GetRandomNegativeNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Reaction randomReaction = CreateRandomModifyReaction(randomDateTimeOffset.AddMinutes(minuteInPast));
            Reaction inputReaction = randomReaction.DeepClone();
            inputReaction.UpdatedDate = randomDateTimeOffset;
            Reaction storageReaction = randomReaction;
            Reaction updatedReaction = inputReaction;
            Reaction expectedReaction = updatedReaction.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateReactionAsync(inputReaction))
                    .ReturnsAsync(updatedReaction);

            // when
            Reaction actualReaction =
                await this.reactionService.ModifyReactionAsync(inputReaction);

            // then
            actualReaction.Should().BeEquivalentTo(expectedReaction);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateReactionAsync(inputReaction),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
