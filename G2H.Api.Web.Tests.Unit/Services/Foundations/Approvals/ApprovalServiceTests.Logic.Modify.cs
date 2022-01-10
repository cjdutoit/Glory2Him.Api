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
        public async Task ShouldModifyApprovalAsync()
        {
            // given
            int minuteInPast = GetRandomNegativeNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Approval randomApproval = CreateRandomModifyApproval(randomDateTimeOffset.AddMinutes(minuteInPast));
            Approval inputApproval = randomApproval.DeepClone();
            inputApproval.UpdatedDate = randomDateTimeOffset;
            Approval storageApproval = randomApproval;
            Approval updatedApproval = inputApproval;
            Approval expectedApproval = updatedApproval.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateApprovalAsync(inputApproval))
                    .ReturnsAsync(updatedApproval);

            // when
            Approval actualApproval =
                await this.approvalService.ModifyApprovalAsync(inputApproval);

            // then
            actualApproval.Should().BeEquivalentTo(expectedApproval);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateApprovalAsync(inputApproval),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
