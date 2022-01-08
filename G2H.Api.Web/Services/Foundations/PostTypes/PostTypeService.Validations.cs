// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using G2H.Api.Web.Models.PostTypes;
using G2H.Api.Web.Models.PostTypes.Exceptions;

namespace G2H.Api.Web.Services.Foundations.PostTypes
{
    public partial class PostTypeService
    {
        private void ValidatePostTypeOnAdd(PostType postType)
        {
            ValidatePostTypeIsNotNull(postType);
        }

        private static void ValidatePostTypeIsNotNull(PostType postType)
        {
            if (postType is null)
            {
                throw new NullPostTypeException();
            }
        }
    }
}
