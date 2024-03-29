﻿// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using G2H.Api.Web.Models.CommentReactions;

namespace G2H.Api.Web.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<CommentReaction> InsertCommentReactionAsync(CommentReaction commentReaction);
        IQueryable<CommentReaction> SelectAllCommentReactions();
        ValueTask<CommentReaction> SelectCommentReactionByIdAsync(Guid commentReactionId);
        ValueTask<CommentReaction> UpdateCommentReactionAsync(CommentReaction commentReaction);
        ValueTask<CommentReaction> DeleteCommentReactionAsync(CommentReaction commentReaction);
    }
}
