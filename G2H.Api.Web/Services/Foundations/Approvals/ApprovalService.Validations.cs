// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System;
using G2H.Api.Web.Models.Approvals;
using G2H.Api.Web.Models.Approvals.Exceptions;
using G2H.Api.Web.Models.Statuses;

namespace G2H.Api.Web.Services.Foundations.Approvals
{
    public partial class ApprovalService
    {
        private void ValidateApprovalOnAdd(Approval approval)
        {
            ValidateApprovalIsNotNull(approval);

            Validate(
                (Rule: IsInvalid(approval.Id), Parameter: nameof(Approval.Id)),
                (Rule: IsInvalid(approval.StatusId), Parameter: nameof(Approval.StatusId)),
                (Rule: IsInvalid(approval.CreatedDate), Parameter: nameof(Approval.CreatedDate)),
                (Rule: IsInvalid(approval.CreatedByUserId), Parameter: nameof(Approval.CreatedByUserId)),
                (Rule: IsInvalid(approval.UpdatedDate), Parameter: nameof(Approval.UpdatedDate)),
                (Rule: IsInvalid(approval.UpdatedByUserId), Parameter: nameof(Approval.UpdatedByUserId)),

                (Rule: IsNotSame(
                    firstDate: approval.UpdatedDate,
                    secondDate: approval.CreatedDate,
                    secondDateName: nameof(Approval.CreatedDate)),
                Parameter: nameof(Approval.UpdatedDate)));
        }

        private static void ValidateApprovalIsNotNull(Approval approval)
        {
            if (approval is null)
            {
                throw new NullApprovalException();
            }
        }

        private static dynamic IsInvalid(Guid id) => new
        {
            Condition = id == Guid.Empty,
            Message = "Id is required"
        };

        private static dynamic IsInvalid(StatusId id) => new
        {
            Condition = id == 0,
            Message = "Id is required"
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

        private static void Validate(params (dynamic Rule, string Parameter)[] validations)
        {
            var invalidApprovalException = new InvalidApprovalException();

            foreach ((dynamic rule, string parameter) in validations)
            {
                if (rule.Condition)
                {
                    invalidApprovalException.UpsertDataList(
                        key: parameter,
                        value: rule.Message);
                }
            }

            invalidApprovalException.ThrowIfContainsErrors();
        }
    }
}
