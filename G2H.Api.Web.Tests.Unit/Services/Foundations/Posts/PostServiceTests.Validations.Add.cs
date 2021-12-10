// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System.Threading.Tasks;
using G2H.Api.Web.Models.Posts;
using G2H.Api.Web.Models.Posts.Exceptions;
using Moq;
using Xunit;

namespace G2H.Api.Web.Tests.Unit.Services.Foundations.Posts
{
    public partial class PostServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfPostIsNullAndLogItAsync()
        {
            // given
            Post nullPost = null;

            var nullPostException =
                new NullPostException();

            var expectedPostValidationException =
                new PostValidationException(nullPostException);

            // when
            ValueTask<Post> addPostTask =
                this.postService.AddPostAsync(nullPost);

            // then
            await Assert.ThrowsAsync<PostValidationException>(() =>
                addPostTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPostValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public async Task ShouldThrowValidationExceptionOnAddIfPostIsInvalidAndLogItAsync(
            string invalidText)
        {
            // given
            var invalidPost = new Post
            {
                Title = invalidText,
                Author = invalidText,
                Content = invalidText
            };

            var invalidPostException =
                new InvalidPostException();

            invalidPostException.AddData(
                key: nameof(Post.Id),
                values: "Id is required");

            invalidPostException.AddData(
                key: nameof(Post.Title),
                values: "Text is required");

            invalidPostException.AddData(
                key: nameof(Post.Author),
                values: "Text is required");

            invalidPostException.AddData(
                key: nameof(Post.Content),
                values: "Text is required");

            invalidPostException.AddData(
                key: nameof(Post.CreatedDate),
                values: "Date is required");

            invalidPostException.AddData(
                key: nameof(Post.CreatedByUserId),
                values: "Id is required");

            invalidPostException.AddData(
                key: nameof(Post.UpdatedDate),
                values: "Date is required");

            invalidPostException.AddData(
                key: nameof(Post.UpdatedByUserId),
                values: "Id is required");

            var expectedPostValidationException =
                new PostValidationException(invalidPostException);

            // when
            ValueTask<Post> addPostTask =
                this.postService.AddPostAsync(invalidPost);

            // then
            var actualException = await Assert.ThrowsAsync<PostValidationException>(() =>
               addPostTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Never());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPostValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertPostAsync(It.IsAny<Post>()),
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
            Post randomPost = CreateRandomPost();
            Post invalidPost = randomPost;

            invalidPost.UpdatedDate =
                invalidPost.CreatedDate.AddDays(randomNumber);

            var invalidPostException =
                new InvalidPostException();

            invalidPostException.AddData(
                key: nameof(Post.UpdatedDate),
                values: $"Date is not the same as {nameof(Post.CreatedDate)}");

            var expectedPostValidationException =
                new PostValidationException(invalidPostException);

            // when
            ValueTask<Post> addPostTask =
                this.postService.AddPostAsync(invalidPost);

            // then
            await Assert.ThrowsAsync<PostValidationException>(() =>
               addPostTask.AsTask());

            this.dateTimeBrokerMock.Verify(broker =>
                broker.GetCurrentDateTimeOffset(),
                    Times.Never());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedPostValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.InsertPostAsync(It.IsAny<Post>()),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
