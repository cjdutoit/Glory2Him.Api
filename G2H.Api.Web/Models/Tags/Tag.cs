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
using G2H.Api.Web.Models.Approvals;
using G2H.Api.Web.Models.Base;
using G2H.Api.Web.Models.PostTags;
using G2H.Api.Web.Models.Users;
using Newtonsoft.Json;

namespace G2H.Api.Web.Models.Tags
{
    public class Tag : IKey, IApproval, IAudit
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Promoted { get; set; }
        public Guid ApprovalId { get; set; }
        public Approval Approval { get; set; }
        public Guid CreatedByUserId { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
        public Guid UpdatedByUserId { get; set; }
        public DateTimeOffset UpdatedDate { get; set; }
        public ApplicationUser CreatedByUser { get; set; }
        public ApplicationUser UpdatedByUser { get; set; }

        [JsonIgnore]
        public virtual List<PostTag> PostTags { get; set; } = new List<PostTag>();
    }
}

