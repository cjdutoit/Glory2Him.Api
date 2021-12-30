// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using G2H.Api.Web.Models.Reactions;
using G2H.Api.Web.Models.Reactions.Exceptions;
using Microsoft.Data.SqlClient;
using Xeptions;

namespace G2H.Api.Web.Services.Foundations.Reactions
{
    public partial class ReactionService
    {
        private delegate ValueTask<Reaction> ReturningReactionFunction();

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
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsReactionException =
                    new AlreadyExistsReactionException(duplicateKeyException);

                throw CreateAndLogDependencyValidationException(alreadyExistsReactionException);
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
    }
}
