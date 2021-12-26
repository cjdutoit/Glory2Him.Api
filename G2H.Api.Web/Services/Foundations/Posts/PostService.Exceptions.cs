// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System.Threading.Tasks;
using G2H.Api.Web.Models.Posts;
using G2H.Api.Web.Models.Posts.Exceptions;
using Microsoft.Data.SqlClient;
using Xeptions;

namespace G2H.Api.Web.Services.Foundations.Posts
{
    public partial class PostService
    {
        private delegate ValueTask<Post> ReturningPostFunction();

        private async ValueTask<Post> TryCatch(ReturningPostFunction returningPostFunction)
        {
            try
            {
                return await returningPostFunction();
            }
            catch (NullPostException nullPostException)
            {
                throw CreateAndLogValidationException(nullPostException);
            }
            catch (InvalidPostException invalidCommentException)
            {
                throw CreateAndLogValidationException(invalidCommentException);
            }
            catch (SqlException sqlException)
            {
                var failedPostStorageException =
                    new FailedPostStorageException(sqlException);

                throw CreateAndLogCriticalDependencyException(failedPostStorageException);
            }
        }

        private PostValidationException CreateAndLogValidationException(
            Xeption exception)
        {
            var postValidationException =
                new PostValidationException(exception);

            this.loggingBroker.LogError(postValidationException);

            return postValidationException;
        }

        private PostDependencyException CreateAndLogCriticalDependencyException(
            Xeption exception)
        {
            var postDependencyException = new PostDependencyException(exception);
            this.loggingBroker.LogCritical(postDependencyException);

            return postDependencyException;
        }
    }
}
