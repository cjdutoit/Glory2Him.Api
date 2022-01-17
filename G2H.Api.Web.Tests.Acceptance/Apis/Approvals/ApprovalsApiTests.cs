// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System;
using G2H.Api.Web.Tests.Acceptance.Brokers;
using G2H.Api.Web.Tests.Acceptance.Models.Approvals;
using Tynamix.ObjectFiller;
using Xunit;

namespace G2H.Api.Web.Tests.Acceptance.Apis.Approvals
{
    [Collection(nameof(ApiTestCollection))]
    public partial class ApprovalsApiTests
    {
        private readonly ApiBroker apiBroker;

        public ApprovalsApiTests(ApiBroker apiBroker) =>
            this.apiBroker = apiBroker;

        private static Approval CreateRandomApproval() =>
            CreateRandomApprovalFiller().Create();

        private static Filler<Approval> CreateRandomApprovalFiller()
        {
            Guid userId = Guid.NewGuid();
            DateTime now = DateTime.UtcNow;
            var filler = new Filler<Approval>();

            filler.Setup()
                .OnProperty(approval => approval.Status).IgnoreIt()
                .OnProperty(approval => approval.CreatedDate).Use(now)
                .OnProperty(approval => approval.CreatedByUserId).Use(userId)
                .OnProperty(approval => approval.UpdatedDate).Use(now)
                .OnProperty(approval => approval.UpdatedByUserId).Use(userId);

            return filler;
        }
    }
}
