// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using Xeptions;

namespace G2H.Api.Web.Models.PostTypes.Exceptions
{
    public class NotFoundPostTypeException : Xeption
    {
        public NotFoundPostTypeException(PostTypeId postTypeId)
            : base(message: $"Couldn't find post type with postTypeId: {postTypeId}.")
        { }
    }
}
