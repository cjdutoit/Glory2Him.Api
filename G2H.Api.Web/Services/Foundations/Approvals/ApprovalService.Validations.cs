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
                Parameter: nameof(Approval.UpdatedDate)),

                (Rule: IsNotSame(
                    firstId: approval.UpdatedByUserId,
                    secondId: approval.CreatedByUserId,
                    secondIdName: nameof(Approval.CreatedByUserId)),
                Parameter: nameof(Approval.UpdatedByUserId)),

                (Rule: IsNotRecent(approval.CreatedDate), Parameter: nameof(Approval.CreatedDate)));
        }

        private void ValidateApprovalOnModify(Approval approval)
        {
            ValidateApprovalIsNotNull(approval);

            Validate(
                (Rule: IsInvalid(approval.Id), Parameter: nameof(Approval.Id)),
                (Rule: IsInvalid(approval.StatusId), Parameter: nameof(Approval.StatusId)),
                (Rule: IsInvalid(approval.CreatedDate), Parameter: nameof(Approval.CreatedDate)),
                (Rule: IsInvalid(approval.CreatedByUserId), Parameter: nameof(Approval.CreatedByUserId)),
                (Rule: IsInvalid(approval.UpdatedDate), Parameter: nameof(Approval.UpdatedDate)),
                (Rule: IsInvalid(approval.UpdatedByUserId), Parameter: nameof(Approval.UpdatedByUserId)),

                (Rule: IsSame(
                    firstDate: approval.UpdatedDate,
                    secondDate: approval.CreatedDate,
                    secondDateName: nameof(Approval.CreatedDate)),
                Parameter: nameof(Approval.UpdatedDate)),

                (Rule: IsNotRecent(approval.UpdatedDate), Parameter: nameof(approval.UpdatedDate)));
        }

        private static void ValidateAgainstStorageApprovalOnModify(Approval inputApproval, Approval storageApproval)
        {
            Validate(
                (Rule: IsNotSame(
                    firstDate: inputApproval.CreatedDate,
                    secondDate: storageApproval.CreatedDate,
                    secondDateName: nameof(Approval.CreatedDate)),
                Parameter: nameof(Approval.CreatedDate)),

                (Rule: IsNotSame(
                    firstId: inputApproval.CreatedByUserId,
                    secondId: storageApproval.CreatedByUserId,
                    secondIdName: nameof(Approval.CreatedByUserId)),
                Parameter: nameof(Approval.CreatedByUserId)),

                (Rule: IsSame(
                    firstDate: inputApproval.UpdatedDate,
                    secondDate: storageApproval.UpdatedDate,
                    secondDateName: nameof(Approval.UpdatedDate)),
                Parameter: nameof(Approval.UpdatedDate)));
        }

        public void ValidateApprovalId(Guid approvalId) =>
            Validate((Rule: IsInvalid(approvalId), Parameter: nameof(Approval.Id)));

        private static void ValidateApprovalIsNotNull(Approval approval)
        {
            if (approval is null)
            {
                throw new NullApprovalException();
            }
        }

        private static void ValidateStorageApproval(Approval maybeApproval, Guid approvalId)
        {
            if (maybeApproval is null)
            {
                throw new NotFoundApprovalException(approvalId);
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
