// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using G2H.Api.Web.Models.Reactions;
using G2H.Api.Web.Models.Reactions.Exceptions;

namespace G2H.Api.Web.Services.Foundations.Reactions
{
    public partial class ReactionService
    {
        private void ValidateReactionOnAdd(Reaction reaction)
        {
            ValidateReactionIsNotNull(reaction);
        }

        private static void ValidateReactionIsNotNull(Reaction reaction)
        {
            if (reaction is null)
            {
                throw new NullReactionException();
            }
        }
    }
}
