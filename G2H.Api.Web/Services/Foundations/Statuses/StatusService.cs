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
using G2H.Api.Web.Brokers.DateTimes;
using G2H.Api.Web.Brokers.Loggings;
using G2H.Api.Web.Brokers.Storages;
using G2H.Api.Web.Models.Statuses;

namespace G2H.Api.Web.Services.Foundations.Statuses
{
    public partial class StatusService : IStatusService
    {
        private readonly IStorageBroker storageBroker;
        private readonly IDateTimeBroker dateTimeBroker;
        private readonly ILoggingBroker loggingBroker;

        public StatusService(
            IStorageBroker storageBroker,
            IDateTimeBroker dateTimeBroker,
            ILoggingBroker loggingBroker)
        {
            this.storageBroker = storageBroker;
            this.dateTimeBroker = dateTimeBroker;
            this.loggingBroker = loggingBroker;
        }

        public ValueTask<Status> AddStatusAsync(Status status) =>
        TryCatch(async () =>
        {
            ValidateStatusOnAdd(status);
            return await this.storageBroker.InsertStatusAsync(status);
        });

        public IQueryable<Status> RetrieveAllStatuses() =>
        TryCatch(() => this.storageBroker.SelectAllStatuses());

        public ValueTask<Status> RetrieveStatusByIdAsync(StatusId statusId) =>
        TryCatch(async () =>
        {
            ValidateStatusId(statusId);

            Status maybeStatus = await this.storageBroker
                .SelectStatusByIdAsync(statusId);

            ValidateStorageStatus(maybeStatus, statusId);

            return maybeStatus;
        });

        public ValueTask<Status> ModifyStatusAsync(Status status) =>
        TryCatch(async () =>
        {
            ValidateStatusOnModify(status);

            Status maybeStatus =
                await this.storageBroker.SelectStatusByIdAsync(status.Id);

            ValidateStorageStatus(maybeStatus, status.Id);
            ValidateAgainstStorageStatusOnModify(inputStatus: status, storageStatus: maybeStatus);

            return await this.storageBroker.UpdateStatusAsync(status);
        });

        public ValueTask<Status> RemoveStatusByIdAsync(StatusId statusId) =>
        TryCatch(async () =>
        {
            ValidateStatusId(statusId);

            Status maybeStatus = await this.storageBroker
                    .SelectStatusByIdAsync(statusId);

            ValidateStorageStatus(maybeStatus, statusId);

            return await this.storageBroker
                    .DeleteStatusAsync(maybeStatus);
        });
    }
}
