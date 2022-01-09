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
using Force.DeepCloner;
using G2H.Api.Web.Models.PostTypes;
using G2H.Api.Web.Models.PostTypes.Exceptions;
using Moq;
using Xunit;

namespace G2H.Api.Web.Tests.Unit.Services.Foundations.PostTypes
{
    public partial class PostTypeServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfPostTypeIsNullAndLogItAsync()
        {
            // given
            PostType nullPostType = null;
            var nullPostTypeException = new NullPostTypeException();

            var expectedPostTypeValidationException =
                new PostTypeValidationException(nullPostTypeException);

            // when
            ValueTask<PostType> modifyPostTypeTask =
                this.postTypeService.ModifyPostTypeAsync(nullPostType);

            // then
            await Assert.ThrowsAsync<PostTypeValidationException>(() =>
                modifyPostTypeTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPostTypeValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdatePostTypeAsync(It.IsAny<PostType>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnModifyIfPostTypeIsInvalidAndLogItAsync(string invalidText)
        {
            // given 
            var invalidPostType = new PostType
            {
                Name = invalidText,
            };

            var invalidPostTypeException = new InvalidPostTypeException();

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
                values:
                new[] {
                    "Date is required",
                    $"Date is the same as {nameof(PostType.CreatedDate)}"
                });

            invalidPostTypeException.AddData(
                key: nameof(PostType.UpdatedByUserId),
                values: "Id is required");

            var expectedPostTypeValidationException =
                new PostTypeValidationException(invalidPostTypeException);

            // when
            ValueTask<PostType> modifyPostTypeTask =
                this.postTypeService.ModifyPostTypeAsync(invalidPostType);

            //then
            await Assert.ThrowsAsync<PostTypeValidationException>(() =>
                modifyPostTypeTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPostTypeValidationException))),
                        Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.UpdatePostTypeAsync(It.IsAny<PostType>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsSameAsCreatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            PostType randomPostType = CreateRandomPostType(randomDateTimeOffset);
            PostType invalidPostType = randomPostType;
            var invalidPostTypeException = new InvalidPostTypeException();

            invalidPostTypeException.AddData(
                key: nameof(PostType.UpdatedDate),
                values: $"Date is the same as {nameof(PostType.CreatedDate)}");

            var expectedPostTypeValidationException =
                new PostTypeValidationException(invalidPostTypeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTimeOffset);

            // when
            ValueTask<PostType> modifyPostTypeTask =
                this.postTypeService.ModifyPostTypeAsync(invalidPostType);

            // then
            await Assert.ThrowsAsync<PostTypeValidationException>(() =>
                modifyPostTypeTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPostTypeValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPostTypeByIdAsync(invalidPostType.Id),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [MemberData(nameof(MinutesBeforeOrAfter))]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsNotRecentAndLogItAsync(int minutes)
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            PostType randomPostType = CreateRandomPostType(randomDateTimeOffset);
            randomPostType.UpdatedDate = randomDateTimeOffset.AddMinutes(minutes);

            var invalidPostTypeException =
                new InvalidPostTypeException();

            invalidPostTypeException.AddData(
                key: nameof(PostType.UpdatedDate),
                values: "Date is not recent");

            var expectedPostTypeValidatonException =
                new PostTypeValidationException(invalidPostTypeException);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                .Returns(randomDateTimeOffset);

            // when
            ValueTask<PostType> modifyPostTypeTask =
                this.postTypeService.ModifyPostTypeAsync(randomPostType);

            // then
            await Assert.ThrowsAsync<PostTypeValidationException>(() =>
                modifyPostTypeTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPostTypeValidatonException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPostTypeByIdAsync(It.IsAny<PostTypeId>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfPostTypeDoesNotExistAndLogItAsync()
        {
            // given
            int randomNegativeMinutes = GetRandomNegativeNumber();
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            PostType randomPostType = CreateRandomPostType(randomDateTimeOffset);
            PostType nonExistPostType = randomPostType;
            nonExistPostType.CreatedDate = randomDateTimeOffset.AddMinutes(randomNegativeMinutes);
            PostType nullPostType = null;

            var notFoundPostTypeException =
                new NotFoundPostTypeException(nonExistPostType.Id);

            var expectedPostTypeValidationException =
                new PostTypeValidationException(notFoundPostTypeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPostTypeByIdAsync(nonExistPostType.Id))
                .ReturnsAsync(nullPostType);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                .Returns(randomDateTimeOffset);

            // when 
            ValueTask<PostType> modifyPostTypeTask =
                this.postTypeService.ModifyPostTypeAsync(nonExistPostType);

            // then
            await Assert.ThrowsAsync<PostTypeValidationException>(() =>
                modifyPostTypeTask.AsTask());

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPostTypeByIdAsync(nonExistPostType.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPostTypeValidationException))),
                        Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageCreatedDateNotSameAsCreatedDateAndLogItAsync()
        {
            // given
            int randomNumber = GetRandomNumber();
            int randomMinutes = randomNumber;
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            PostType randomPostType = CreateRandomModifyPostType(randomDateTimeOffset);
            PostType invalidPostType = randomPostType;
            PostType storagePostType = randomPostType.DeepClone();
            invalidPostType.CreatedDate = storagePostType.CreatedDate.AddMinutes(randomMinutes);
            var invalidPostTypeException = new InvalidPostTypeException();

            invalidPostTypeException.AddData(
                key: nameof(PostType.CreatedDate),
                values: $"Date is not the same as {nameof(PostType.CreatedDate)}");

            var expectedPostTypeValidationException =
                new PostTypeValidationException(invalidPostTypeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPostTypeByIdAsync(invalidPostType.Id))
                .ReturnsAsync(storagePostType);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                .Returns(randomDateTimeOffset);

            // when
            ValueTask<PostType> modifyPostTypeTask =
                this.postTypeService.ModifyPostTypeAsync(invalidPostType);

            // then
            await Assert.ThrowsAsync<PostTypeValidationException>(() =>
                modifyPostTypeTask.AsTask());

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPostTypeByIdAsync(invalidPostType.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogError(It.Is(SameExceptionAs(
                   expectedPostTypeValidationException))),
                       Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageCreatedUserIdNotSameAsCreatedUserIdAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            PostType randomPostType = CreateRandomModifyPostType(randomDateTimeOffset);
            PostType invalidPostType = randomPostType;
            PostType storagePostType = randomPostType.DeepClone();
            invalidPostType.CreatedByUserId = Guid.NewGuid();
            var invalidPostTypeException = new InvalidPostTypeException();

            invalidPostTypeException.AddData(
                key: nameof(PostType.CreatedByUserId),
                values: $"Id is not the same as {nameof(PostType.CreatedByUserId)}");

            var expectedPostTypeValidationException =
                new PostTypeValidationException(invalidPostTypeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPostTypeByIdAsync(invalidPostType.Id))
                .ReturnsAsync(storagePostType);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                .Returns(randomDateTimeOffset);

            // when
            ValueTask<PostType> modifyPostTypeTask =
                this.postTypeService.ModifyPostTypeAsync(invalidPostType);

            // then
            await Assert.ThrowsAsync<PostTypeValidationException>(() =>
                modifyPostTypeTask.AsTask());

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPostTypeByIdAsync(invalidPostType.Id),
                    Times.Once);

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
               broker.LogError(It.Is(SameExceptionAs(
                   expectedPostTypeValidationException))),
                       Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfStorageUpdatedDateSameAsUpdatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            PostType randomPostType = CreateRandomModifyPostType(randomDateTimeOffset);
            PostType invalidPostType = randomPostType;

            PostType storagePostType = randomPostType.DeepClone();
            invalidPostType.UpdatedDate = storagePostType.UpdatedDate;

            var invalidPostTypeException = new InvalidPostTypeException();

            invalidPostTypeException.AddData(
                key: nameof(PostType.UpdatedDate),
                values: $"Date is the same as {nameof(PostType.UpdatedDate)}");

            var expectedPostTypeValidationException =
                new PostTypeValidationException(invalidPostTypeException);

            this.storageBrokerMock.Setup(broker =>
                broker.SelectPostTypeByIdAsync(invalidPostType.Id))
                .ReturnsAsync(storagePostType);

            this.dateTimeBrokerMock.Setup(broker =>
                broker.GetCurrentDateTimeOffset())
                    .Returns(randomDateTimeOffset);

            // when
            ValueTask<PostType> modifyPostTypeTask =
                this.postTypeService.ModifyPostTypeAsync(invalidPostType);

            // then
            await Assert.ThrowsAsync<PostTypeValidationException>(() =>
                modifyPostTypeTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Once);

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPostTypeValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectPostTypeByIdAsync(invalidPostType.Id),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
