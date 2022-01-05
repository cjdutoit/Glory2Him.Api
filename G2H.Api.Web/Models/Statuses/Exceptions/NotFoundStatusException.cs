// ---------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// See License.txt in the project root for license information.
// John 3:16 NIV "For God so loved the world that he gave his one and only Son, that whoever believes in him shall not perish but have eternal life."
// https://john.bible/john-3-16
// ---------------------------------------------------------------

using Xeptions;

namespace G2H.Api.Web.Models.Statuses.Exceptions
{
    public class NotFoundStatusException : Xeption
    {
        public NotFoundStatusException(StatusId statusId)
            : base(message: $"Couldn't find status with statusId: {statusId}.")
        { }
    }
}
