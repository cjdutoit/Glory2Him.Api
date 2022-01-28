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

        private async ValueTask<Approval> PostRandomApprovalAsync()
        {
            Approval randomApproval = CreateRandomApproval();
            await this.apiBroker.PostApprovalAsync(randomApproval);

            return randomApproval;
        }

        private async ValueTask<List<Approval>> PostRandomApprovalsAsync()
        {
            int randomNumber = GetRandomNumber();
            var randomApprovals = new List<Approval>();

            for (int i = 0; i < randomNumber; i++)
            {
                randomApprovals.Add(await PostRandomApprovalAsync());
            }
            return randomApprovals;
        }

        private int GetRandomNumber() =>
            new IntRange(min: 2, max: 10).GetValue();

        private static DateTimeOffset GetRandomDateTime() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static Approval UpdateApprovalWithRandomValues(Approval inputApproval)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;

            var filler = new Filler<Approval>();

            filler.Setup()
                .OnProperty(approval => approval.Id).Use(inputApproval.Id)
                .OnProperty(approval => approval.CreatedDate).Use(inputApproval.CreatedDate)
                .OnProperty(approval => approval.CreatedByUserId).Use(inputApproval.CreatedByUserId)
                .OnProperty(approval => approval.UpdatedDate).Use(now)
                .OnType<DateTimeOffset>().Use(GetRandomDateTime())
                .OnProperty(approval => approval.Status).IgnoreIt();

            return filler.Create();
        }

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
