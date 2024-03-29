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
using G2H.Api.Web.Models.Statuses;

namespace G2H.Api.Web.Services.Foundations.Statuses
{
    public interface IStatusService
    {
        ValueTask<Status> AddStatusAsync(Status status);
        IQueryable<Status> RetrieveAllStatuses();
        ValueTask<Status> RetrieveStatusByIdAsync(StatusId statusId);
        ValueTask<Status> ModifyStatusAsync(Status status);
        ValueTask<Status> RemoveStatusByIdAsync(StatusId statusId);
    }
}
