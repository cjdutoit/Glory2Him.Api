// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using G2H.Api.Web.Tests.Acceptance.Models.Statuses;

namespace G2H.Api.Web.Tests.Acceptance.Models.Bases
{
    public interface IStatus
    {
        StatusId StatusId { get; set; }
        Status Status { get; set; }
    }
}
