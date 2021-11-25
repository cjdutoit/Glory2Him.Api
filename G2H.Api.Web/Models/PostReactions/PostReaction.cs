// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System;
using G2H.Api.Web.Models.Base;
using G2H.Api.Web.Models.Posts;
using G2H.Api.Web.Models.Reactions;

namespace G2H.Api.Web.Models.PostReactions
{
    public class PostReaction : IAudit
    {
        public Guid PostId { get; set; }
        public virtual Post Post { get; set; }

        public int ReactionId { get; set; }
        public virtual Reaction Reaction { get; set; }

        public Guid UserId { get; set; }

        public Guid CreatedByUserId { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public Guid UpdatedByUserId { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
    }
}
