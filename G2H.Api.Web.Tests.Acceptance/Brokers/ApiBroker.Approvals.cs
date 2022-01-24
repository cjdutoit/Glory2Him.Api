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
using System.Threading.Tasks;
using G2H.Api.Web.Tests.Acceptance.Models.Approvals;

namespace G2H.Api.Web.Tests.Acceptance.Brokers
{
    public partial class ApiBroker
    {
        private const string ApprovalsRelativeUrl = "api/approvals";

        public async ValueTask<Approval> PostApprovalAsync(Approval approval) =>
            await this.apiFactoryClient.PostContentAsync(ApprovalsRelativeUrl, approval);

        public async ValueTask<Approval> GetApprovalByIdAsync(Guid approvalId) =>
            await this.apiFactoryClient.GetContentAsync<Approval>($"{ApprovalsRelativeUrl}/{approvalId}");

        public async ValueTask<List<Approval>> GetAllApprovalsAsync() =>
          await this.apiFactoryClient.GetContentAsync<List<Approval>>($"{ApprovalsRelativeUrl}/");

        public async ValueTask<Approval> PutApprovalAsync(Approval approval) =>
            await this.apiFactoryClient.PutContentAsync(ApprovalsRelativeUrl, approval);

        public async ValueTask<Approval> DeleteApprovalByIdAsync(Guid approvalId) =>
            await this.apiFactoryClient.DeleteContentAsync<Approval>($"{ApprovalsRelativeUrl}/{approvalId}");
    }
}
