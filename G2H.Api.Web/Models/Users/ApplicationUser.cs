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
using Microsoft.AspNetCore.Identity;

namespace G2H.Api.Web.Models.Users
{
    public class ApplicationUser : IdentityUser<Guid>
    {

        public virtual List<Approval> Approvals { get; set; } = new List<Approval>();
    }
}
