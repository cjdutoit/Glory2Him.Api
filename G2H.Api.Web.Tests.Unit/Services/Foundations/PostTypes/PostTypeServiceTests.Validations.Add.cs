// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System.Threading.Tasks;
using G2H.Api.Web.Models.PostTypes;
using G2H.Api.Web.Models.PostTypes.Exceptions;
using Moq;
using Xunit;

namespace G2H.Api.Web.Tests.Unit.Services.Foundations.PostTypes
{
    public partial class PostTypeServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfPostTypeIsNullAndLogItAsync()
        {
            // given
            PostType nullPostType = null;

            var nullPostTypeException =
                new NullPostTypeException();

            var expectedPostTypeValidationException =
                new PostTypeValidationException(nullPostTypeException);

            // when
            ValueTask<PostType> addPostTypeTask =
                this.postTypeService.AddPostTypeAsync(nullPostType);

            // then
            await Assert.ThrowsAsync<PostTypeValidationException>(() =>
                addPostTypeTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPostTypeValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
