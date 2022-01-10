// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System.Threading.Tasks;
using G2H.Api.Web.Models.Approvals;
using G2H.Api.Web.Models.Approvals.Exceptions;
using Xeptions;

namespace G2H.Api.Web.Services.Foundations.Approvals
{
    public partial class ApprovalService
    {
        private delegate ValueTask<Approval> ReturningApprovalFunction();

        private async ValueTask<Approval> TryCatch(ReturningApprovalFunction returningApprovalFunction)
        {
            try
            {
                return await returningApprovalFunction();
            }
            catch (NullApprovalException nullApprovalException)
            {
                throw CreateAndLogValidationException(nullApprovalException);
            }
            catch (InvalidApprovalException invalidApprovalException)
            {
                throw CreateAndLogValidationException(invalidApprovalException);
            }
        }

        private ApprovalValidationException CreateAndLogValidationException(
            Xeption exception)
        {
            var approvalValidationException =
                new ApprovalValidationException(exception);

            this.loggingBroker.LogError(approvalValidationException);

            return approvalValidationException;
        }
    }
}
