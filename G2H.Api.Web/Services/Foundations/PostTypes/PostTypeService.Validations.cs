// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System;
using G2H.Api.Web.Models.PostTypes;
using G2H.Api.Web.Models.PostTypes.Exceptions;

namespace G2H.Api.Web.Services.Foundations.PostTypes
{
    public partial class PostTypeService
    {
        private void ValidatePostTypeOnAdd(PostType postType)
        {
            ValidatePostTypeIsNotNull(postType);

            Validate(
               (Rule: IsInvalid(postType.Id), Parameter: nameof(PostType.Id)),
               (Rule: IsInvalid(postType.Name), Parameter: nameof(PostType.Name)),
               (Rule: IsInvalid(postType.CreatedDate), Parameter: nameof(PostType.CreatedDate)),
               (Rule: IsInvalid(postType.CreatedByUserId), Parameter: nameof(PostType.CreatedByUserId)),
               (Rule: IsInvalid(postType.UpdatedDate), Parameter: nameof(PostType.UpdatedDate)),
               (Rule: IsInvalid(postType.UpdatedByUserId), Parameter: nameof(PostType.UpdatedByUserId)));
        }

        private static void ValidatePostTypeIsNotNull(PostType postType)
        {
            if (postType is null)
            {
                throw new NullPostTypeException();
            }
        }

        private static dynamic IsInvalid(PostTypeId id) => new
        {
            Condition = id == 0,
            Message = "Id is required"
        };

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
            var invalidPostTypeException = new InvalidPostTypeException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidPostTypeException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidPostTypeException.ThrowIfContainsErrors();
        }
    }
}
