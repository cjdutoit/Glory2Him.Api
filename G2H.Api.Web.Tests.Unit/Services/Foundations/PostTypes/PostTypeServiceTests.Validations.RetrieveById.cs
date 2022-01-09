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
        public async Task ShouldThrowValidationExceptionOnRetrieveByIdIfIdIsInvalidAndLogItAsync()
        {
            // given
            var invalidPostTypeId = (PostTypeId)default;

            var invalidPostTypeException =
                new InvalidPostTypeException();

            invalidPostTypeException.AddData(
                key: nameof(PostType.Id),
                values: "Id is required");

            var expectedPostTypeValidationException =
                new PostTypeValidationException(invalidPostTypeException);

            // when
            ValueTask<PostType> retrievePostTypeByIdTask =
                this.postTypeService.RetrievePostTypeByIdAsync(invalidPostTypeId);

            // then
            await Assert.ThrowsAsync<PostTypeValidationException>(() =>
                retrievePostTypeByIdTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPostTypeValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPostTypeByIdAsync(It.IsAny<PostTypeId>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowNotFoundExceptionOnRetrieveByIdIfPostTypeIsNotFoundAndLogItAsync()
        {
            //given
            PostTypeId somePostTypeId = GetRandomPostTypeId();
            PostType noPostType = null;

            var notFoundPostTypeException =
                new NotFoundPostTypeException(somePostTypeId);

            var expectedPostTypeValidationException =
                new PostTypeValidationException(notFoundPostTypeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPostTypeByIdAsync(It.IsAny<PostTypeId>()))
                    .ReturnsAsync(noPostType);

            //when
            ValueTask<PostType> retrievePostTypeByIdTask =
                this.postTypeService.RetrievePostTypeByIdAsync(somePostTypeId);

            //then
            await Assert.ThrowsAsync<PostTypeValidationException>(() =>
               retrievePostTypeByIdTask.AsTask());

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPostTypeByIdAsync(It.IsAny<PostTypeId>()),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPostTypeValidationException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
        }
    }
}