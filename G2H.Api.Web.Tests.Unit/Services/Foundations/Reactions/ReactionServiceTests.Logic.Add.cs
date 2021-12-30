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
        public async Task ShouldAddReactionAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset =
                GetRandomDateTimeOffset();

            Reaction randomReaction = CreateRandomReaction(randomDateTimeOffset);
            Reaction inputReaction = randomReaction;
            Reaction storageReaction = inputReaction;
            Reaction expectedReaction = storageReaction.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.InsertReactionAsync(inputReaction))
                    .ReturnsAsync(storageReaction);

            // when
            Reaction actualReaction = await this.reactionService
                .AddReactionAsync(inputReaction);

            // then
            actualReaction.Should().BeEquivalentTo(expectedReaction);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertReactionAsync(inputReaction),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
