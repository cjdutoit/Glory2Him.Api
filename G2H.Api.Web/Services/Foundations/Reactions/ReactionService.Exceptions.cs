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
using G2H.Api.Web.Models.Reactions;
using G2H.Api.Web.Models.Reactions.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace G2H.Api.Web.Services.Foundations.Reactions
{
    public partial class ReactionService
    {
        private delegate ValueTask<Reaction> ReturningReactionFunction();
        private delegate IQueryable<Reaction> ReturningReactionsFunction();

        private async ValueTask<Reaction> TryCatch(ReturningReactionFunction returningReactionFunction)
        {
            try
            {
                return await returningReactionFunction();
            }
            catch (NullReactionException nullReactionException)
            {
                throw CreateAndLogValidationException(nullReactionException);
            }
            catch (InvalidReactionException invalidReactionException)
            {
                throw CreateAndLogValidationException(invalidReactionException);
            }
            catch (SqlException sqlException)
            {
                var failedReactionStorageException =
                    new FailedReactionStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedReactionStorageException);
            }
            catch (NotFoundReactionException notFoundReactionException)
            {
                throw CreateAndLogValidationException(notFoundReactionException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsReactionException =
                    new AlreadyExistsReactionException(duplicateKeyException);

                throw CreateAndLogDependencyValidationException(alreadyExistsReactionException);
            }
            catch (DbUpdateConcurrencyException dbUpdateConcurrencyException)
            {
                var lockedReactionException = new LockedReactionException(dbUpdateConcurrencyException);

                throw CreateAndLogDependencyValidationException(lockedReactionException);
            }
            catch (DbUpdateException databaseUpdateException)
            {
                var failedReactionStorageException =
                    new FailedReactionStorageException(databaseUpdateException);

                throw CreateAndLogDependecyException(failedReactionStorageException);
            }
            catch (Exception exception)
            {
                var failedReactionServiceException =
                    new FailedReactionServiceException(exception);

                throw CreateAndLogServiceException(failedReactionServiceException);
            }
        }

        private IQueryable<Reaction> TryCatch(ReturningReactionsFunction returningReactionsFunction)
        {
            try
            {
                return returningReactionsFunction();
            }
            catch (SqlException sqlException)
            {
                var failedReactionStorageException =
                    new FailedReactionStorageException(sqlException);
                throw CreateAndLogCriticalDependencyException(failedReactionStorageException);
            }
            catch (Exception exception)
            {
                var failedReactionServiceException =
                    new FailedReactionServiceException(exception);

                throw CreateAndLogServiceException(failedReactionServiceException);
            }
        }

        private ReactionValidationException CreateAndLogValidationException(
            Xeption exception)
        {
            var reactionValidationException =
                new ReactionValidationException(exception);

            this.loggingBroker.LogError(reactionValidationException);

            return reactionValidationException;
        }

        private ReactionDependencyException CreateAndLogCriticalDependencyException(
            Xeption exception)
        {
            var reactionDependencyException = new ReactionDependencyException(exception);
            this.loggingBroker.LogCritical(reactionDependencyException);

            return reactionDependencyException;
        }

        private ReactionDependencyValidationException CreateAndLogDependencyValidationException(
            Xeption exception)
        {
            var reactionDependencyValidationException =
                new ReactionDependencyValidationException(exception);

            this.loggingBroker.LogError(reactionDependencyValidationException);

            return reactionDependencyValidationException;
        }

        private ReactionDependencyException CreateAndLogDependecyException(
            Xeption exception)
        {
            var reactionDependencyException = new ReactionDependencyException(exception);
            this.loggingBroker.LogError(reactionDependencyException);

            return reactionDependencyException;
        }

        private ReactionServiceException CreateAndLogServiceException(
            Xeption exception)
        {
            var reactionServiceException = new ReactionServiceException(exception);
            this.loggingBroker.LogError(reactionServiceException);

            return reactionServiceException;
        }
    }
}
