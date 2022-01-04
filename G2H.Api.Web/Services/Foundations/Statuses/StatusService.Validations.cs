// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System;
using G2H.Api.Web.Models.Statuses;
using G2H.Api.Web.Models.Statuses.Exceptions;

namespace G2H.Api.Web.Services.Foundations.Statuses
{
    public partial class StatusService
    {
        private void ValidateStatusOnAdd(Status status)
        {
            ValidateStatusIsNotNull(status);
            Validate(
                (Rule: IsInvalid(status.Id), Parameter: nameof(Status.Id)),
                (Rule: IsInvalid(status.Name), Parameter: nameof(Status.Name)),
                (Rule: IsInvalid(status.CreatedDate), Parameter: nameof(Status.CreatedDate)),
                (Rule: IsInvalid(status.CreatedByUserId), Parameter: nameof(Status.CreatedByUserId)),
                (Rule: IsInvalid(status.UpdatedDate), Parameter: nameof(Status.UpdatedDate)),
                (Rule: IsInvalid(status.UpdatedByUserId), Parameter: nameof(Status.UpdatedByUserId)));
        }

        private static void ValidateStatusIsNotNull(Status status)
        {
            if (status is null)
            {
                throw new NullStatusException();
            }
        }

        private static dynamic IsInvalid(StatusId id) => new
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
            var invalidStatusException = new InvalidStatusException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidStatusException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidStatusException.ThrowIfContainsErrors();
        }
    }
}
