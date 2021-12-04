// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using G2H.Api.Web.Models.Base;
using G2H.Api.Web.Models.PostReactions;
using G2H.Api.Web.Models.PostsComments;
using G2H.Api.Web.Models.PostTags;
using G2H.Api.Web.Models.PostTypes;
using G2H.Api.Web.Models.Statuses;
using G2H.Api.Web.Models.Users;
using Newtonsoft.Json;

namespace G2H.Api.Web.Models.Posts
{
    public class Post : IKey, IStatus, IAudit
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Content { get; set; }

        public PostTypeId PostTypeId { get; set; }
        public virtual PostType PostType { get; set; }

        public bool IsCommentsAllowed { get; set; }
        public bool IsCommentsVisible { get; set; }

        public StatusId StatusId { get; set; }
        public Status Status { get; set; }

        public Guid CreatedByUserId { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public Guid UpdatedByUserId { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }

        public ApplicationUser CreatedByUser { get; set; }
        public ApplicationUser UpdatedByUser { get; set; }

        [JsonIgnore]
        public virtual List<PostReaction> PostReactions { get; set; } = new List<PostReaction>();
        [JsonIgnore]
        public virtual List<PostTag> PostTags { get; set; } = new List<PostTag>();
        [JsonIgnore]
        public virtual List<PostComment> PostComments { get; set; } = new List<PostComment>();
        [JsonIgnore]
        public virtual List<PostAttachment> PostAttachments { get; set; } = new List<PostAttachment>();

    }
}

