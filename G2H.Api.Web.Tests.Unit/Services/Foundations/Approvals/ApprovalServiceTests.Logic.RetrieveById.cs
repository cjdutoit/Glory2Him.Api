// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System.Threading.Tasks;
using FluentAssertions;
using Force.DeepCloner;
using G2H.Api.Web.Models.Approvals;
using Moq;
using Xunit;

namespace G2H.Api.Web.Tests.Unit.Services.Foundations.Approvals
{
    public partial class ApprovalServiceTests
    {
        [Fact]
        public async Task ShouldRetrieveApprovalByIdAsync()
        {
            // given
            Approval randomApproval = CreateRandomApproval();
            Approval inputApproval = randomApproval;
            Approval storageApproval = randomApproval;
            Approval expectedApproval = storageApproval.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectApprovalByIdAsync(inputApproval.Id))
                    .ReturnsAsync(storageApproval);

            // when
            Approval actualApproval =
                await this.approvalService.RetrieveApprovalByIdAsync(inputApproval.Id);

            // then
            actualApproval.Should().BeEquivalentTo(expectedApproval);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectApprovalByIdAsync(inputApproval.Id),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
