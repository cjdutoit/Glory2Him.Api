// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System;
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
        public async Task ShouldRemoveApprovalByIdAsync()
        {
            // given
            Guid randomId = Guid.NewGuid();
            Guid inputApprovalId = randomId;
            Approval randomApproval = CreateRandomApproval();
            Approval storageApproval = randomApproval;
            Approval expectedInputApproval = storageApproval;
            Approval deletedApproval = expectedInputApproval;
            Approval expectedApproval = deletedApproval.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectApprovalByIdAsync(inputApprovalId))
                    .ReturnsAsync(storageApproval);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteApprovalAsync(expectedInputApproval))
                    .ReturnsAsync(deletedApproval);

            // when
            Approval actualApproval = await this.approvalService
                .RemoveApprovalByIdAsync(inputApprovalId);

            // then
            actualApproval.Should().BeEquivalentTo(expectedApproval);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectApprovalByIdAsync(inputApprovalId),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteApprovalAsync(expectedInputApproval),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
