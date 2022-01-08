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
using G2H.Api.Web.Models.PostTypes;
using Moq;
using Xunit;

namespace G2H.Api.Web.Tests.Unit.Services.Foundations.PostTypes
{
    public partial class PostTypeServiceTests
    {
        [Fact]
        public void ShouldReturnPostTypes()
        {
            // given
            IQueryable<PostType> randomPostTypes = CreateRandomPostTypes();
            IQueryable<PostType> storagePostTypes = randomPostTypes;
            IQueryable<PostType> expectedPostTypes = storagePostTypes;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllPostTypes())
                    .Returns(storagePostTypes);

            // when
            IQueryable<PostType> actualPostTypes =
                this.postTypeService.RetrieveAllPostTypes();

            // then
            actualPostTypes.Should().BeEquivalentTo(expectedPostTypes);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllPostTypes(),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
