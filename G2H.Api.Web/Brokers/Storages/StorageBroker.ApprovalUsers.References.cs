// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using G2H.Api.Web.Models.ApprovalUsers;
using Microsoft.EntityFrameworkCore;

namespace G2H.Api.Web.Brokers.Storages
{
    public partial class StorageBroker
    {
        private static void AddApprovalUserReferences(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ApprovalUser>()
                .HasOne(approvalUser => approvalUser.Approval)
                .WithMany(approval => approval.ApprovalUsers)
                .HasForeignKey(approvalUser => approvalUser.ApprovalId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
