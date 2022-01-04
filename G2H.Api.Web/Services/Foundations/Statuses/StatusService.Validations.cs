// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using G2H.Api.Web.Models.Statuses;
using G2H.Api.Web.Models.Statuses.Exceptions;

namespace G2H.Api.Web.Services.Foundations.Statuses
{
    public partial class StatusService
    {
        private void ValidateStatusOnAdd(Status status)
        {
            ValidateStatusIsNotNull(status);
        }

        private static void ValidateStatusIsNotNull(Status status)
        {
            if (status is null)
            {
                throw new NullStatusException();
            }
        }
    }
}
