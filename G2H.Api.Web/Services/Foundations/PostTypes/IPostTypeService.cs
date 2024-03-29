﻿// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using G2H.Api.Web.Models.PostTypes;

namespace G2H.Api.Web.Services.Foundations.PostTypes
{
    public interface IPostTypeService
    {
        ValueTask<PostType> AddPostTypeAsync(PostType postType);
        IQueryable<PostType> RetrieveAllPostTypes();
        ValueTask<PostType> RetrievePostTypeByIdAsync(PostTypeId postTypeId);
        ValueTask<PostType> ModifyPostTypeAsync(PostType postType);
    }
}
