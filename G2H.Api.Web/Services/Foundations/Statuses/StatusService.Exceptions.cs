// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System.Threading.Tasks;
using G2H.Api.Web.Models.Statuses;
using G2H.Api.Web.Models.Statuses.Exceptions;
using Xeptions;

namespace G2H.Api.Web.Services.Foundations.Statuses
{
    public partial class StatusService
    {
        private delegate ValueTask<Status> ReturningStatusFunction();

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
        }

        private StatusValidationException CreateAndLogValidationException(
            Xeption exception)
        {
            var statusValidationException =
                new StatusValidationException(exception);

            this.loggingBroker.LogError(statusValidationException);

            return statusValidationException;
        }
    }
}
