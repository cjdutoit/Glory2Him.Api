// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System.Linq;
using FluentAssertions;
using G2H.Api.Web.Models.Approvals;
using Moq;
using Xunit;

namespace G2H.Api.Web.Tests.Unit.Services.Foundations.Approvals
{
    public partial class ApprovalServiceTests
    {
        [Fact]
        public void ShouldReturnApprovals()
        {
            // given
            IQueryable<Approval> randomApprovals = CreateRandomApprovals();
            IQueryable<Approval> storageApprovals = randomApprovals;
            IQueryable<Approval> expectedApprovals = storageApprovals;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllApprovals())
                    .Returns(storageApprovals);

            // when
            IQueryable<Approval> actualApprovals =
                this.approvalService.RetrieveAllApprovals();

            // then
            actualApprovals.Should().BeEquivalentTo(expectedApprovals);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllApprovals(),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
