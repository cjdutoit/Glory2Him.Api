﻿// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using G2H.Api.Web.Models.Reactions;

namespace G2H.Api.Web.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<Reaction> InsertReactionAsync(Reaction reaction);
        IQueryable<Reaction> SelectAllReactions();
        ValueTask<Reaction> SelectReactionByIdAsync(ReactionId reactionId);
        ValueTask<Reaction> UpdateReactionAsync(Reaction reaction);
        ValueTask<Reaction> DeleteReactionAsync(Reaction reaction);
    }
}
