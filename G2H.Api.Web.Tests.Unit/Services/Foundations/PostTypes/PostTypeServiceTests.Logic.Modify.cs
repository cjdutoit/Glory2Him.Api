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
using G2H.Api.Web.Models.PostTypes;
using Moq;
using Xunit;

namespace G2H.Api.Web.Tests.Unit.Services.Foundations.PostTypes
{
    public partial class PostTypeServiceTests
    {
        [Fact]
        public async Task ShouldModifyPostTypeAsync()
        {
            // given
            int minuteInPast = GetRandomNegativeNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            PostType randomPostType = CreateRandomModifyPostType(randomDateTimeOffset.AddMinutes(minuteInPast));
            PostType inputPostType = randomPostType.DeepClone();
            inputPostType.UpdatedDate = randomDateTimeOffset;
            PostType storagePostType = randomPostType;
            PostType updatedPostType = inputPostType;
            PostType expectedPostType = updatedPostType.DeepClone();

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTimeOffset);

            this.storageBrokerMock.Setup(broker =>
                broker.UpdatePostTypeAsync(inputPostType))
                    .ReturnsAsync(updatedPostType);

            // when
            PostType actualPostType =
                await this.postTypeService.ModifyPostTypeAsync(inputPostType);

            // then
            actualPostType.Should().BeEquivalentTo(expectedPostType);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdatePostTypeAsync(inputPostType),
                    Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
