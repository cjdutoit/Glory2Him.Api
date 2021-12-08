// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using G2H.Api.Web.Models.Attachments;

namespace G2H.Api.Web.Brokers.Storages
{
    public partial interface IStorageBroker
    {
        ValueTask<Attachment> InsertAttachmentAsync(Attachment attachment);
        IQueryable<Attachment> SelectAllAttachments();
        ValueTask<Attachment> SelectAttachmentByIdAsync(Guid attachmentId);
        ValueTask<Attachment> UpdateAttachmentAsync(Attachment attachment);
        ValueTask<Attachment> DeleteAttachmentAsync(Attachment attachment);
    }
}
