// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System;
using G2H.Api.Web.Models.Posts;
using G2H.Api.Web.Models.Posts.Exceptions;
using G2H.Api.Web.Models.PostTypes;

namespace G2H.Api.Web.Services.Foundations.Posts
{
    public partial class PostService
    {
        private void ValidatePostOnAdd(Post post)
        {
            ValidatePostIsNotNull(post);

            Validate(
                (Rule: IsInvalid(post.Id), Parameter: nameof(Post.Id)),
                (Rule: IsInvalid(post.Title), Parameter: nameof(Post.Title)),
                (Rule: IsInvalid(post.Author), Parameter: nameof(Post.Author)),
                (Rule: IsInvalid(post.Content), Parameter: nameof(Post.Content)),
                (Rule: IsInvalid(post.CreatedDate), Parameter: nameof(Post.CreatedDate)),
                (Rule: IsInvalid(post.CreatedByUserId), Parameter: nameof(Post.CreatedByUserId)),
                (Rule: IsInvalid(post.UpdatedDate), Parameter: nameof(Post.UpdatedDate)),
                (Rule: IsInvalid(post.UpdatedByUserId), Parameter: nameof(Post.UpdatedByUserId)),

                (Rule: IsGreaterThan(
                    text: post.Content,
                    maxLength: 280,
                    condition: post.PostTypeId == PostTypeId.Quote),
                Parameter: nameof(Post.Content)),

                (Rule: IsGreaterThan(
                    text: post.Content,
                    maxLength: 2200,
                    condition: post.PostTypeId == PostTypeId.Story),
                Parameter: nameof(Post.Content)),

                (Rule: IsNotSame(
                    firstDate: post.UpdatedDate,
                    secondDate: post.CreatedDate,
                    secondDateName: nameof(Post.CreatedDate)),
                Parameter: nameof(Post.UpdatedDate)),

                (Rule: IsNotSame(
                    firstId: post.UpdatedByUserId,
                    secondId: post.CreatedByUserId,
                    secondIdName: nameof(Post.CreatedByUserId)),
                Parameter: nameof(Post.UpdatedByUserId)),

                (Rule: IsNotRecent(post.CreatedDate), Parameter: nameof(post.CreatedDate)));
        }

        private void ValidatePostOnModify(Post post)
        {
            ValidatePostIsNotNull(post);

            Validate(
                (Rule: IsInvalid(post.Id), Parameter: nameof(Post.Id)),
                (Rule: IsInvalid(post.Title), Parameter: nameof(Post.Title)),
                (Rule: IsInvalid(post.Author), Parameter: nameof(Post.Author)),
                (Rule: IsInvalid(post.Content), Parameter: nameof(Post.Content)),
                (Rule: IsInvalid(post.CreatedDate), Parameter: nameof(Post.CreatedDate)),
                (Rule: IsInvalid(post.CreatedByUserId), Parameter: nameof(Post.CreatedByUserId)),
                (Rule: IsInvalid(post.UpdatedDate), Parameter: nameof(Post.UpdatedDate)),
                (Rule: IsInvalid(post.UpdatedByUserId), Parameter: nameof(Post.UpdatedByUserId)),

                (Rule: IsGreaterThan(
                    text: post.Content,
                    maxLength: 280,
                    condition: post.PostTypeId == PostTypeId.Quote),
                Parameter: nameof(Post.Content)),

                (Rule: IsGreaterThan(
                    text: post.Content,
                    maxLength: 2200,
                    condition: post.PostTypeId == PostTypeId.Story),
                Parameter: nameof(Post.Content)),

                (Rule: IsSame(
                    firstDate: post.UpdatedDate,
                    secondDate: post.CreatedDate,
                    secondDateName: nameof(Post.CreatedDate)),
                Parameter: nameof(Post.UpdatedDate)),

                (Rule: IsNotRecent(post.UpdatedDate), Parameter: nameof(post.UpdatedDate)));
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is required"
        };

        private static dynamic IsInvalid(string text) => new
        {
            Condition = String.IsNullOrWhiteSpace(text),
            Message = "Text is required"
        };

        private static dynamic IsInvalid(DateTimeOffset date) => new
        {
            Condition = date == default,
            Message = "Date is required"
        };

        private static dynamic IsGreaterThan(
            string text,
            int maxLength,
            bool condition) => new
            {
                Condition = condition && (text ?? "").Length > maxLength,
                Message = $"Text is exceeding character limit"
            };

        private static dynamic IsNotSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate != secondDate,
                Message = $"Date is not the same as {secondDateName}"
            };

        private static dynamic IsNotSame(
            Guid firstId,
            Guid secondId,
            string secondIdName) => new
            {
                Condition = firstId != secondId,
                Message = $"Id is not the same as {secondIdName}"
            };

        private static dynamic IsSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate == secondDate,
                Message = $"Date is the same as {secondDateName}"
            };

        private dynamic IsNotRecent(DateTimeOffset date) => new
        {
            Condition = IsDateNotRecent(date),
            Message = "Date is not recent"
        };

        private bool IsDateNotRecent(DateTimeOffset date)
        {
            DateTimeOffset currentDateTime =
                this.dateTimeBroker.GetCurrentDateTimeOffset();

            TimeSpan timeDifference = currentDateTime.Subtract(date);
            TimeSpan oneMinute = TimeSpan.FromMinutes(1);

            return timeDifference.Duration() > oneMinute;
        }

        public void ValidatePostId(Guid postId) =>
            Validate((Rule: IsInvalid(postId), Parameter: nameof(Post.Id)));

        private static void ValidateStoragePost(Post maybePost, Guid postId)
        {
            if (maybePost is null)
            {
                throw new NotFoundPostException(postId);
            }
        }

        private static void ValidateAgainstStoragePostOnModify(Post inputPost, Post storagePost)
        {
            Validate(
                (Rule: IsNotSame(
                    firstDate: inputPost.CreatedDate,
                    secondDate: storagePost.CreatedDate,
                    secondDateName: nameof(Post.CreatedDate)),
                Parameter: nameof(Post.CreatedDate)),

                (Rule: IsNotSame(
                    firstId: inputPost.CreatedByUserId,
                    secondId: storagePost.CreatedByUserId,
                    secondIdName: nameof(Post.CreatedByUserId)),
                Parameter: nameof(Post.CreatedByUserId)),

                (Rule: IsSame(
                    firstDate: inputPost.UpdatedDate,
                    secondDate: storagePost.UpdatedDate,
                    secondDateName: nameof(Post.UpdatedDate)),
                Parameter: nameof(Post.UpdatedDate)));
        }

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidCommentException = new InvalidPostException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidCommentException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidCommentException.ThrowIfContainsErrors();
        }

        private static void ValidatePostIsNotNull(Post post)
        {
            if (post is null)
            {
                throw new NullPostException();
            }
        }
    }
}
