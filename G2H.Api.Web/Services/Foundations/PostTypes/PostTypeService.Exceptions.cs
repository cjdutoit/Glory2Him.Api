﻿// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System.Threading.Tasks;
using EFxceptions.Models.Exceptions;
using G2H.Api.Web.Models.PostTypes;
using G2H.Api.Web.Models.PostTypes.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Xeptions;

namespace G2H.Api.Web.Services.Foundations.PostTypes
{
    public partial class PostTypeService
    {
        private delegate ValueTask<PostType> ReturningPostTypeFunction();

        private async ValueTask<PostType> TryCatch(ReturningPostTypeFunction returningPostTypeFunction)
        {
            try
            {
                return await returningPostTypeFunction();
            }
            catch (NullPostTypeException nullPostTypeException)
            {
                throw CreateAndLogValidationException(nullPostTypeException);
            }
            catch (InvalidPostTypeException invalidPostTypeException)
            {
                throw CreateAndLogValidationException(invalidPostTypeException);
            }
            catch (SqlException sqlException)
            {
                var failedPostTypeStorageException =
                    new FailedPostTypeStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedPostTypeStorageException);
            }
            catch (DuplicateKeyException duplicateKeyException)
            {
                var alreadyExistsPostTypeException =
                    new AlreadyExistsPostTypeException(duplicateKeyException);

                throw CreateAndLogDependencyValidationException(alreadyExistsPostTypeException);
            }
            catch (DbUpdateException databaseUpdateException)
            {
                var failedPostTypeStorageException =
                    new FailedPostTypeStorageException(databaseUpdateException);

                throw CreateAndLogDependecyException(failedPostTypeStorageException);
            }
        }

        private PostTypeValidationException CreateAndLogValidationException(
            Xeption exception)
        {
            var postTypeValidationException =
                new PostTypeValidationException(exception);

            this.loggingBroker.LogError(postTypeValidationException);

            return postTypeValidationException;
        }

        private PostTypeDependencyException CreateAndLogCriticalDependencyException(
           Xeption exception)
        {
            var postTypeDependencyException = new PostTypeDependencyException(exception);
            this.loggingBroker.LogCritical(postTypeDependencyException);

            return postTypeDependencyException;
        }

        private PostTypeDependencyValidationException CreateAndLogDependencyValidationException(
        Xeption exception)
        {
            var postTypeDependencyValidationException =
                new PostTypeDependencyValidationException(exception);

            this.loggingBroker.LogError(postTypeDependencyValidationException);

            return postTypeDependencyValidationException;
        }

        private PostTypeDependencyException CreateAndLogDependecyException(
            Xeption exception)
        {
            var postTypeDependencyException = new PostTypeDependencyException(exception);
            this.loggingBroker.LogError(postTypeDependencyException);

            return postTypeDependencyException;
        }
    }
}
