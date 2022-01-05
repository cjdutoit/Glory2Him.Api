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
        public async Task ShouldRetrieveStatusByIdAsync()
        {
            // given
            Status randomStatus = CreateRandomStatus();
            Status inputStatus = randomStatus;
            Status storageStatus = randomStatus;
            Status expectedStatus = storageStatus.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectStatusByIdAsync(inputStatus.Id))
                    .ReturnsAsync(storageStatus);

            // when
            Status actualStatus =
                await this.statusService.RetrieveStatusByIdAsync(inputStatus.Id);

            // then
            actualStatus.Should().BeEquivalentTo(expectedStatus);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectStatusByIdAsync(inputStatus.Id),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
