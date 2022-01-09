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
using G2H.Api.Web.Models.PostTypes;
using Moq;
using Xunit;

namespace G2H.Api.Web.Tests.Unit.Services.Foundations.PostTypes
{
    public partial class PostTypeServiceTests
    {
        [Fact]
        public async Task ShouldRetrievePostTypeByIdAsync()
        {
            // given
            PostType randomPostType = CreateRandomPostType();
            PostType inputPostType = randomPostType;
            PostType storagePostType = randomPostType;
            PostType expectedPostType = storagePostType.DeepClone();

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPostTypeByIdAsync(inputPostType.Id))
                    .ReturnsAsync(storagePostType);

            // when
            PostType actualPostType =
                await this.postTypeService.RetrievePostTypeByIdAsync(inputPostType.Id);

            // then
            actualPostType.Should().BeEquivalentTo(expectedPostType);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPostTypeByIdAsync(inputPostType.Id),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
