// --------------------------------------------------------------------------------
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
using G2H.Api.Web.Models.Approvals;

namespace G2H.Api.Web.Services.Foundations.Approvals
{
    public interface IApprovalService
    {
        ValueTask<Approval> AddApprovalAsync(Approval approval);
        IQueryable<Approval> RetrieveAllApprovals();
        ValueTask<Approval> RetrieveApprovalByIdAsync(Guid approvalId);
    }
}
