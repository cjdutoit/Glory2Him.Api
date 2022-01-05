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
using G2H.Api.Web.Models.Statuses;
using G2H.Api.Web.Models.Statuses.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace G2H.Api.Web.Services.Foundations.Statuses
{
    public partial class StatusService
    {
        private delegate ValueTask<Status> ReturningStatusFunction();
        private delegate IQueryable<Status> ReturningStatusesFunction();

        private async ValueTask<Status> TryCatch(ReturningStatusFunction returningStatusFunction)
        {
            try
            {
                return await returningStatusFunction();
            }
            catch (NullStatusException nullStatusException)
            {
                throw CreateAndLogValidationException(nullStatusException);
            }
            catch (InvalidStatusException invalidStatusException)
            {
                throw CreateAndLogValidationException(invalidStatusException);
            }
            catch (SqlException sqlException)
            {
                var failedStatusStorageException =
                    new FailedStatusStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedStatusStorageException);
            }
            catch (NotFoundStatusException notFoundStatusException)
            {
                throw CreateAndLogValidationException(notFoundStatusException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsStatusException =
                    new AlreadyExistsStatusException(duplicateKeyException);

                throw CreateAndLogDependencyValidationException(alreadyExistsStatusException);
            }
            catch (DbUpdateException databaseUpdateException)
            {
                var failedStatusStorageException =
                    new FailedStatusStorageException(databaseUpdateException);

                throw CreateAndLogDependecyException(failedStatusStorageException);
            }
            catch (Exception exception)
            {
                var failedStatusServiceException =
                    new FailedStatusServiceException(exception);

                throw CreateAndLogServiceException(failedStatusServiceException);
            }
        }

        private IQueryable<Status> TryCatch(ReturningStatusesFunction returningStatusesFunction)
        {
            try
            {
                return returningStatusesFunction();
            }
            catch (SqlException sqlException)
            {
                var failedStatusStorageException =
                    new FailedStatusStorageException(sqlException);
                throw CreateAndLogCriticalDependencyException(failedStatusStorageException);
            }
            catch (Exception exception)
            {
                var failedStatusServiceException =
                    new FailedStatusServiceException(exception);

                throw CreateAndLogServiceException(failedStatusServiceException);
            }
        }

        private StatusValidationException CreateAndLogValidationException(
            Xeption exception)
        {
            var statusValidationException =
                new StatusValidationException(exception);

            this.loggingBroker.LogError(statusValidationException);

            return statusValidationException;
        }

        private StatusDependencyException CreateAndLogCriticalDependencyException(
            Xeption exception)
        {
            var statusDependencyException = new StatusDependencyException(exception);
            this.loggingBroker.LogCritical(statusDependencyException);

            return statusDependencyException;
        }

        private StatusDependencyValidationException CreateAndLogDependencyValidationException(
            Xeption exception)
        {
            var statusDependencyValidationException =
                new StatusDependencyValidationException(exception);

            this.loggingBroker.LogError(statusDependencyValidationException);

            return statusDependencyValidationException;
        }

        private StatusDependencyException CreateAndLogDependecyException(
            Xeption exception)
        {
            var statusDependencyException = new StatusDependencyException(exception);
            this.loggingBroker.LogError(statusDependencyException);

            return statusDependencyException;
        }

        private StatusServiceException CreateAndLogServiceException(
            Xeption exception)
        {
            var statusServiceException = new StatusServiceException(exception);
            this.loggingBroker.LogError(statusServiceException);

            return statusServiceException;
        }
    }
}
