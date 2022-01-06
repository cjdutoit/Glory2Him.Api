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
using G2H.Api.Web.Models.Statuses;
using Moq;
using Xunit;

namespace G2H.Api.Web.Tests.Unit.Services.Foundations.Statuses
{
    public partial class StatusServiceTests
    {
        [Fact]
        public async Task ShouldRemoveStatusByIdAsync()
        {
            // given
            StatusId randomId = GetRandomStatusId();
            StatusId inputStatusId = randomId;
            Status randomStatus = CreateRandomStatus();
            Status storageStatus = randomStatus;
            Status expectedInputStatus = storageStatus;
            Status deletedStatus = expectedInputStatus;
            Status expectedStatus = deletedStatus.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectStatusByIdAsync(inputStatusId))
                    .ReturnsAsync(storageStatus);

            this.storageBrokerMock.Setup(broker =>
                broker.DeleteStatusAsync(expectedInputStatus))
                    .ReturnsAsync(deletedStatus);

            // when
            Status actualStatus = await this.statusService
                .RemoveStatusByIdAsync(inputStatusId);

            // then
            actualStatus.Should().BeEquivalentTo(expectedStatus);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectStatusByIdAsync(inputStatusId),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.DeleteStatusAsync(expectedInputStatus),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}
