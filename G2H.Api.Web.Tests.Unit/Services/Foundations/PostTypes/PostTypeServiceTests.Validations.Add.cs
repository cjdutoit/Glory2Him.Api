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

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfPostTypeIsInvalidAndLogItAsync(
            string invalidText)
        {
            // given
            var invalidPostType = new PostType
            {
                Name = invalidText
            };

            var invalidPostTypeException =
                new InvalidPostTypeException();

            invalidPostTypeException.AddData(
                key: nameof(PostType.Id),
                values: "Id is required");

            invalidPostTypeException.AddData(
                key: nameof(PostType.Name),
                values: "Text is required");

            invalidPostTypeException.AddData(
                key: nameof(PostType.CreatedDate),
                values: "Date is required");

            invalidPostTypeException.AddData(
                key: nameof(PostType.CreatedByUserId),
                values: "Id is required");

            invalidPostTypeException.AddData(
                key: nameof(PostType.UpdatedDate),
                values: "Date is required");

            invalidPostTypeException.AddData(
                key: nameof(PostType.UpdatedByUserId),
                values: "Id is required");

            var expectedPostTypeValidationException =
                new PostTypeValidationException(invalidPostTypeException);

            // when
            ValueTask<PostType> addPostTypeTask =
                this.postTypeService.AddPostTypeAsync(invalidPostType);

            // then
            await Assert.ThrowsAsync<PostTypeValidationException>(() =>
               addPostTypeTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPostTypeValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertPostTypeAsync(It.IsAny<PostType>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCreateAndUpdateDatesIsNotSameAndLogItAsync()
        {
            // given
            int randomNumber = GetRandomNumber();
            PostType randomPostType = CreateRandomPostType();
            PostType invalidPostType = randomPostType;

            invalidPostType.UpdatedDate =
                invalidPostType.CreatedDate.AddDays(randomNumber);

            var invalidPostTypeException =
                new InvalidPostTypeException();

            invalidPostTypeException.AddData(
                key: nameof(PostType.UpdatedDate),
                values: $"Date is not the same as {nameof(PostType.CreatedDate)}");

            var expectedPostTypeValidationException =
                new PostTypeValidationException(invalidPostTypeException);

            // when
            ValueTask<PostType> addPostTypeTask =
                this.postTypeService.AddPostTypeAsync(invalidPostType);

            // then
            await Assert.ThrowsAsync<PostTypeValidationException>(() =>
               addPostTypeTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPostTypeValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertPostTypeAsync(It.IsAny<PostType>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfCreateAndUpdateUserIdsIsNotSameAndLogItAsync()
        {
            // given
            int randomNumber = GetRandomNumber();
            PostType randomPostType = CreateRandomPostType();
            PostType invalidPostType = randomPostType;

            invalidPostType.UpdatedByUserId = Guid.NewGuid();

            var invalidPostTypeException =
                new InvalidPostTypeException();

            invalidPostTypeException.AddData(
                key: nameof(PostType.UpdatedByUserId),
                values: $"Id is not the same as {nameof(PostType.CreatedByUserId)}");

            var expectedPostTypeValidationException =
                new PostTypeValidationException(invalidPostTypeException);

            // when
            ValueTask<PostType> addPostTypeTask =
                this.postTypeService.AddPostTypeAsync(invalidPostType);

            // then
            var ex = await Assert.ThrowsAsync<PostTypeValidationException>(() =>
               addPostTypeTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPostTypeValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertPostTypeAsync(It.IsAny<PostType>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(MinutesBeforeOrAfter))]
        public async Task ShouldThrowValidationExceptionOnAddIfCreatedDateIsNotRecentAndLogItAsync(
            int minutesBeforeOrAfter)
        {
            // given
            DateTimeOffset randomDateTimeOffset =
                GetRandomDateTimeOffset();

            DateTimeOffset invalidDateTime =
                randomDateTimeOffset.AddMinutes(minutesBeforeOrAfter);

            PostType randomPostType = CreateRandomPostType(invalidDateTime);
            PostType invalidPostType = randomPostType;

            var invalidPostTypeException =
                new InvalidPostTypeException();

            invalidPostTypeException.AddData(
                key: nameof(PostType.CreatedDate),
                values: "Date is not recent");

            var expectedPostTypeValidationException =
                new PostTypeValidationException(invalidPostTypeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTimeOffset);

            // when
            ValueTask<PostType> addPostTypeTask =
                this.postTypeService.AddPostTypeAsync(invalidPostType);

            // then
            await Assert.ThrowsAsync<PostTypeValidationException>(() =>
               addPostTypeTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPostTypeValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertPostTypeAsync(It.IsAny<PostType>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
