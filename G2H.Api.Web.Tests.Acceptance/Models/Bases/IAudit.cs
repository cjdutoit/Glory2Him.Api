// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System;
using G2H.Api.Web.Tests.Acceptance.Models.Users;

namespace G2H.Api.Web.Tests.Acceptance.Models.Bases
{
    public interface IAudit
    {
        Guid CreatedByUserId { get; set; }

        DateTimeOffset CreatedDate { get; set; }

        Guid UpdatedByUserId { get; set; }

        DateTimeOffset UpdatedDate { get; set; }

        ApplicationUser CreatedByUser { get; set; }

        ApplicationUser UpdatedByUser { get; set; }
    }
}
