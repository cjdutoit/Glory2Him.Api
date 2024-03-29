﻿// --------------------------------------------------------------------------------
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
                (Rule: IsInvalid(reaction.UpdatedByUserId), Parameter: nameof(Reaction.UpdatedByUserId)),

                (Rule: IsNotSame(
                    firstDate: reaction.UpdatedDate,
                    secondDate: reaction.CreatedDate,
                    secondDateName: nameof(Reaction.CreatedDate)),
                Parameter: nameof(Reaction.UpdatedDate)),

                (Rule: IsNotSame(
                    firstId: reaction.UpdatedByUserId,
                    secondId: reaction.CreatedByUserId,
                    secondIdName: nameof(Reaction.CreatedByUserId)),
                Parameter: nameof(Reaction.UpdatedByUserId)),

                (Rule: IsNotRecent(reaction.CreatedDate), Parameter: nameof(Reaction.CreatedDate)));
        }

        private void ValidateReactionOnModify(Reaction reaction)
        {
            ValidateReactionIsNotNull(reaction);

            Validate(
                (Rule: IsInvalid(reaction.Id), Parameter: nameof(Reaction.Id)),
                (Rule: IsInvalid(reaction.Name), Parameter: nameof(Reaction.Name)),
                (Rule: IsInvalid(reaction.CreatedDate), Parameter: nameof(Reaction.CreatedDate)),
                (Rule: IsInvalid(reaction.CreatedByUserId), Parameter: nameof(Reaction.CreatedByUserId)),
                (Rule: IsInvalid(reaction.UpdatedDate), Parameter: nameof(Reaction.UpdatedDate)),
                (Rule: IsInvalid(reaction.UpdatedByUserId), Parameter: nameof(Reaction.UpdatedByUserId)),

                (Rule: IsSame(
                    firstDate: reaction.UpdatedDate,
                    secondDate: reaction.CreatedDate,
                    secondDateName: nameof(Reaction.CreatedDate)),
                Parameter: nameof(Reaction.UpdatedDate)),

                (Rule: IsNotRecent(reaction.UpdatedDate), Parameter: nameof(reaction.UpdatedDate)));
        }

        private static void ValidateReactionIsNotNull(Reaction reaction)
        {
            if (reaction is null)
            {
                throw new NullReactionException();
            }
        }

        public void ValidateReactionId(ReactionId reactionId) =>
            Validate((Rule: IsInvalid(reactionId), Parameter: nameof(Reaction.Id)));

        private static void ValidateStorageReaction(Reaction maybeReaction, ReactionId reactionId)
        {
            if (maybeReaction is null)
            {
                throw new NotFoundReactionException(reactionId);
            }
        }

        private static void ValidateAgainstStorageReactionOnModify(Reaction inputReaction, Reaction storageReaction)
        {
            Validate(
                (Rule: IsNotSame(
                    firstDate: inputReaction.CreatedDate,
                    secondDate: storageReaction.CreatedDate,
                    secondDateName: nameof(Reaction.CreatedDate)),
                Parameter: nameof(Reaction.CreatedDate)),

                (Rule: IsNotSame(
                    firstId: inputReaction.CreatedByUserId,
                    secondId: storageReaction.CreatedByUserId,
                    secondIdName: nameof(Reaction.CreatedByUserId)),
                Parameter: nameof(Reaction.CreatedByUserId)),

                (Rule: IsSame(
                    firstDate: inputReaction.UpdatedDate,
                    secondDate: storageReaction.UpdatedDate,
                    secondDateName: nameof(Reaction.UpdatedDate)),
                Parameter: nameof(Reaction.UpdatedDate)));
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

        private static dynamic IsNotSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate != secondDate,
                Message = $"Date is not the same as {secondDateName}"
            };

        private static dynamic IsSame(
            DateTimeOffset firstDate,
            DateTimeOffset secondDate,
            string secondDateName) => new
            {
                Condition = firstDate == secondDate,
                Message = $"Date is the same as {secondDateName}"
            };

        private static dynamic IsNotSame(
            Guid firstId,
            Guid secondId,
            string secondIdName) => new
            {
                Condition = firstId != secondId,
                Message = $"Id is not the same as {secondIdName}"
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
