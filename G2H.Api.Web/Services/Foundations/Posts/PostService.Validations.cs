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
                (Rule: IsInvalid(post.UpdatedByUserId), Parameter: nameof(Post.UpdatedByUserId)));
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
