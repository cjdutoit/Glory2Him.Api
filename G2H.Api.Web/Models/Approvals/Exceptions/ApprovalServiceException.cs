// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System;
using Xeptions;

namespace G2H.Api.Web.Models.Approvals.Exceptions
{
    public class ApprovalServiceException : Xeption
    {
        public ApprovalServiceException(Exception innerException)
            : base(message: "Approval service error occurred, contact support.", innerException)
        { }
    }
}
