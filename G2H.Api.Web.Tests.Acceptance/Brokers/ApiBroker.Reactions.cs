// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Threading.Tasks;
using G2H.Api.Web.Tests.Acceptance.Models.Reactions;

namespace G2H.Api.Web.Tests.Acceptance.Brokers
{
    public partial class ApiBroker
    {
        private const string ReactionsRelativeUrl = "api/reactions";

        public async ValueTask<List<Reaction>> GetAllReactionsAsync() =>
            await this.apiFactoryClient.GetContentAsync<List<Reaction>>($"{ReactionsRelativeUrl}/");

        public async ValueTask<Reaction> GetReactionByIdAsync(ReactionId reactionId) =>
            await this.apiFactoryClient.GetContentAsync<Reaction>($"{ReactionsRelativeUrl}/{reactionId}");
    }
}
