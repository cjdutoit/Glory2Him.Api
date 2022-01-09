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
using System.Text.Json.Serialization;
using G2H.Api.Web.Models.ApprovalUsers;
using G2H.Api.Web.Models.Attachments;
using G2H.Api.Web.Models.Base;
using G2H.Api.Web.Models.Comments;
using G2H.Api.Web.Models.Posts;
using G2H.Api.Web.Models.PostTags;
using G2H.Api.Web.Models.Statuses;
using G2H.Api.Web.Models.Tags;

namespace G2H.Api.Web.Models.Approvals
{
    public class Approval : IKey, IStatus, IAudit, IVersioning
    {
        public Guid Id { get; set; }
        public StatusId StatusId { get; set; }
        public Status Status { get; set; }
        public Guid BusinessKey { get; set; }
        public int Version { get; set; }
        public bool IsAuditRecord { get; set; }
        public Guid CreatedByUserId { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public Guid UpdatedByUserId { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }

        public List<ApprovalUser> ApprovalUsers { get; set; } = new List<ApprovalUser>();

        [JsonIgnore]
        public virtual List<Post> Posts { get; set; } = new List<Post>();
        [JsonIgnore]
        public virtual List<Comment> Comments { get; set; } = new List<Comment>();
        [JsonIgnore]
        public virtual List<Attachment> Attachments { get; set; } = new List<Attachment>();
        [JsonIgnore]
        public virtual List<PostTag> PostTags { get; set; } = new List<PostTag>();
        [JsonIgnore]
        public virtual List<Tag> Tags { get; set; } = new List<Tag>();
    }
}
