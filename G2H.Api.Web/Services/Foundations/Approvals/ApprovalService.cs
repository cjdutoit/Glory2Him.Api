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
using G2H.Api.Web.Brokers.DateTimes;
using G2H.Api.Web.Brokers.Loggings;
using G2H.Api.Web.Brokers.Storages;
using G2H.Api.Web.Models.Approvals;

namespace G2H.Api.Web.Services.Foundations.Approvals
{
    public partial class ApprovalService : IApprovalService
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public ApprovalService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<Approval> AddApprovalAsync(Approval approval) =>
        TryCatch(async () =>
        {
            ValidateApprovalOnAdd(approval);

            return await this.storageBroker.InsertApprovalAsync(approval);
        });

        public IQueryable<Approval> RetrieveAllApprovals() =>
        TryCatch(() => this.storageBroker.SelectAllApprovals());

        public ValueTask<Approval> RetrieveApprovalByIdAsync(Guid approvalId) =>
        TryCatch(async () =>
        {
            ValidateApprovalId(approvalId);

            Approval maybeApproval = await this.storageBroker
                .SelectApprovalByIdAsync(approvalId);

            ValidateStorageApproval(maybeApproval, approvalId);

            return maybeApproval;
        });

        public ValueTask<Approval> ModifyApprovalAsync(Approval approval) =>
        TryCatch(async () =>
        {
            ValidateApprovalOnModify(approval);

            Approval maybeApproval =
                await this.storageBroker.SelectApprovalByIdAsync(approval.Id);

            ValidateStorageApproval(maybeApproval, approval.Id);
            ValidateAgainstStorageApprovalOnModify(inputApproval: approval, storageApproval: maybeApproval);

            return await this.storageBroker.UpdateApprovalAsync(approval);
        });

        public ValueTask<Approval> RemoveApprovalByIdAsync(Guid approvalId) =>
            throw new NotImplementedException();
    }
}
