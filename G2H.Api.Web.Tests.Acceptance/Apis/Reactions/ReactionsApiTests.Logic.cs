// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using G2H.Api.Web.Tests.Acceptance.Models.Reactions;
using Xunit;

namespace G2H.Api.Web.Tests.Acceptance.Apis.Reactions
{
    public partial class ReactionsApiTests
    {
        [Fact]
        public async Task ShouldGetAllReactionsAsync()
        {
            // given
            List<Reaction> reactions = await CreateReactionsAsync();
            List<Reaction> expectedReactions = reactions;

            // when
            List<Reaction> actualReactions = await this.apiBroker.GetAllReactionsAsync();

            // then
            foreach (Reaction expectedReaction in expectedReactions)
            {
                Reaction actualReaction = actualReactions.Single(status => status.Id == expectedReaction.Id);
                actualReaction.Name.Should().Be(expectedReaction.Name);
            }
        }

        [Fact]
        public async Task ShouldGetReactionAsync()
        {
            // given
            await CreateReactionsAsync();
            Reaction randomReaction = GetRandomReaction();
            Reaction expectedReaction = randomReaction;

            // when
            Reaction actualReaction = await this.apiBroker.GetReactionByIdAsync(expectedReaction.Id);

            // then
            actualReaction.Name.Should().Be(expectedReaction.Name);
        }
    }
}
