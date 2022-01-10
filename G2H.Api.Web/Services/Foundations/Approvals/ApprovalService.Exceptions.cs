// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using G2H.Api.Web.Models.Approvals;
using G2H.Api.Web.Models.Approvals.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace G2H.Api.Web.Services.Foundations.Approvals
{
    public partial class ApprovalService
    {
        private delegate ValueTask<Approval> ReturningApprovalFunction();
        private delegate IQueryable<Approval> ReturningApprovalsFunction();

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
            catch (SqlException sqlException)
            {
                var failedApprovalStorageException =
                    new FailedApprovalStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedApprovalStorageException);
            }
            catch (NotFoundApprovalException notFoundApprovalException)
            {
                throw CreateAndLogValidationException(notFoundApprovalException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsApprovalException =
                    new AlreadyExistsApprovalException(duplicateKeyException);

                throw CreateAndLogDependencyValidationException(alreadyExistsApprovalException);
            }
            catch (ForeignKeyConstraintConflictException foreignKeyConstraintConflictException)
            {
                var invalidApprovalReferenceException =
                    new InvalidApprovalReferenceException(foreignKeyConstraintConflictException);

                throw CreateAndLogDependencyValidationException(invalidApprovalReferenceException);
            }
            catch (DbUpdateException databaseUpdateException)
            {
                var failedApprovalStorageException =
                    new FailedApprovalStorageException(databaseUpdateException);

                throw CreateAndLogDependecyException(failedApprovalStorageException);
            }
            catch (Exception exception)
            {
                var failedApprovalServiceException =
                    new FailedApprovalServiceException(exception);

                throw CreateAndLogServiceException(failedApprovalServiceException);
            }
        }

        private IQueryable<Approval> TryCatch(ReturningApprovalsFunction returningApprovalsFunction)
        {
            try
            {
                return returningApprovalsFunction();
            }
            catch (SqlException sqlException)
            {
                var failedApprovalStorageException =
                    new FailedApprovalStorageException(sqlException);
                throw CreateAndLogCriticalDependencyException(failedApprovalStorageException);
            }
            catch (Exception exception)
            {
                var failedApprovalServiceException =
                    new FailedApprovalServiceException(exception);

                throw CreateAndLogServiceException(failedApprovalServiceException);
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

        private ApprovalDependencyException CreateAndLogCriticalDependencyException(
            Xeption exception)
        {
            var approvalDependencyException = new ApprovalDependencyException(exception);
            this.loggingBroker.LogCritical(approvalDependencyException);

            return approvalDependencyException;
        }

        private ApprovalDependencyValidationException CreateAndLogDependencyValidationException(
        Xeption exception)
        {
            var approvalDependencyValidationException =
                new ApprovalDependencyValidationException(exception);

            this.loggingBroker.LogError(approvalDependencyValidationException);

            return approvalDependencyValidationException;
        }

        private ApprovalDependencyException CreateAndLogDependecyException(
            Xeption exception)
        {
            var approvalDependencyException = new ApprovalDependencyException(exception);
            this.loggingBroker.LogError(approvalDependencyException);

            return approvalDependencyException;
        }

        private ApprovalServiceException CreateAndLogServiceException(
            Xeption exception)
        {
            var approvalServiceException = new ApprovalServiceException(exception);
            this.loggingBroker.LogError(approvalServiceException);

            return approvalServiceException;
        }
    }
}
