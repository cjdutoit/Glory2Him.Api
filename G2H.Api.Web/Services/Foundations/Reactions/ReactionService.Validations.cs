// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System;
using G2H.Api.Web.Models.Reactions;
using G2H.Api.Web.Models.Reactions.Exceptions;

namespace G2H.Api.Web.Services.Foundations.Reactions
{
    public partial class ReactionService
    {
        private void ValidateReactionOnAdd(Reaction reaction)
        {
            ValidateReactionIsNotNull(reaction);

            Validate(
                (Rule: IsInvalid(reaction.Id), Parameter: nameof(Reaction.Id)),
                (Rule: IsInvalid(reaction.Name), Parameter: nameof(Reaction.Name)),
                (Rule: IsInvalid(reaction.CreatedDate), Parameter: nameof(Reaction.CreatedDate)),
                (Rule: IsInvalid(reaction.CreatedByUserId), Parameter: nameof(Reaction.CreatedByUserId)),
                (Rule: IsInvalid(reaction.UpdatedDate), Parameter: nameof(Reaction.UpdatedDate)),
                (Rule: IsInvalid(reaction.UpdatedByUserId), Parameter: nameof(Reaction.UpdatedByUserId)));
        }

        private static void ValidateReactionIsNotNull(Reaction reaction)
        {
            if (reaction is null)
            {
                throw new NullReactionException();
            }
        }

        private static dynamic IsInvalid(ReactionId id) => new
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
            var invalidReactionException = new InvalidReactionException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidReactionException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidReactionException.ThrowIfContainsErrors();
        }
    }
}
