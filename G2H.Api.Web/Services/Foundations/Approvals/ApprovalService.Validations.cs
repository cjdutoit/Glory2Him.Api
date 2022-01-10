// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using G2H.Api.Web.Models.Approvals;
using G2H.Api.Web.Models.Approvals.Exceptions;

namespace G2H.Api.Web.Services.Foundations.Approvals
{
    public partial class ApprovalService
    {
        private void ValidateApprovalOnAdd(Approval approval)
        {
            ValidateApprovalIsNotNull(approval);
        }

        private static void ValidateApprovalIsNotNull(Approval approval)
        {
            if (approval is null)
            {
                throw new NullApprovalException();
            }
        }
    }
}
