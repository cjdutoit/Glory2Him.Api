// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System;
using G2H.Api.Web.Models.Comments;

namespace G2H.Api.Web.Models.PostsComments
{
    public class CommentComment
    {
        public Guid ParentCommentId { get; set; }
        public Comment ParentComment { get; set; }
        public Guid ChildCommentId { get; set; }
        public Comment ChildComment { get; set; }
    }
}
