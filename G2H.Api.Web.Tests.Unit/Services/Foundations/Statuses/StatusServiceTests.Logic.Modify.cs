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
using G2H.Api.Web.Models.Statuses;
using Moq;
using Xunit;

namespace G2H.Api.Web.Tests.Unit.Services.Foundations.Statuses
{
    public partial class StatusServiceTests
    {
        [Fact]
        public async Task ShouldModifyStatusAsync()
        {
            // given
            int minuteInPast = GetRandomNegativeNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Status randomStatus = CreateRandomModifyStatus(randomDateTimeOffset.AddMinutes(minuteInPast));
            Status inputStatus = randomStatus.DeepClone();
            inputStatus.UpdatedDate = randomDateTimeOffset;
            Status storageStatus = randomStatus;
            Status updatedStatus = inputStatus;
            Status expectedStatus = updatedStatus.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectStatusByIdAsync(inputStatus.Id))
                    .ReturnsAsync(storageStatus);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdateStatusAsync(inputStatus))
                    .ReturnsAsync(updatedStatus);

            // when
            Status actualStatus =
                await this.statusService.ModifyStatusAsync(inputStatus);

            // then
            actualStatus.Should().BeEquivalentTo(expectedStatus);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateStatusAsync(inputStatus),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
