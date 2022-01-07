// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using G2H.Api.Web.Tests.Acceptance.Brokers;
using Microsoft.OpenApi.Extensions;
using Tynamix.ObjectFiller;
using Xunit;

namespace G2H.Api.Web.Tests.Acceptance.Apis.Statuses
{
    [Collection(nameof(ApiTestCollection))]
    public partial class StatusesApiTests
    {
        private readonly ApiBroker apiBroker;

        public StatusesApiTests(ApiBroker apiBroker)
        {
            this.apiBroker = apiBroker;
        }


        private async ValueTask<List<Models.Statuses.Status>> CreateStatusesAsync()
        {
            foreach (var item in GetStorageStatuses())
            {
                if (!apiBroker.StatusService.RetrieveAllStatuses().Any(status => status.Id == item.Id))
                {
                    await apiBroker.StatusService.AddStatusAsync(item);
                }
            }

            return GetClientStatuses();
        }

        private static DateTimeOffset GetRandomDateTimeOffset() =>
            new DateTimeRange(earliestDate: new DateTime()).GetValue();

        private static List<Web.Models.Statuses.Status> GetStorageStatuses()
        {
            List<Web.Models.Statuses.Status> statuses = new List<Web.Models.Statuses.Status>();


            foreach (Web.Models.Statuses.StatusId statusId in Enum.GetValues(typeof(Web.Models.Statuses.StatusId)))
            {
                var dateTimeOffset = GetRandomDateTimeOffset();
                var userId = Guid.NewGuid();

                var filler = new Filler<Web.Models.Statuses.Status>();

                filler.Setup()
                    .OnProperty(status => status.Id).Use(statusId)
                    .OnProperty(status => status.Name).Use(statusId.GetDisplayName())
                    .OnType<DateTimeOffset>().Use(dateTimeOffset)
                    .OnProperty(status => status.CreatedByUserId).Use(userId)
                    .OnProperty(status => status.CreatedByUser).IgnoreIt()
                    .OnProperty(status => status.UpdatedByUserId).Use(userId)
                    .OnProperty(status => status.UpdatedByUser).IgnoreIt()
                    .OnProperty(status => status.Approvals).IgnoreIt();

                statuses.Add(filler.Create());
            }

            return statuses;
        }

        private static List<Models.Statuses.Status> GetClientStatuses()
        {
            List<Models.Statuses.Status> statuses = new List<Models.Statuses.Status>();


            foreach (Models.Statuses.StatusId statusId in Enum.GetValues(typeof(Models.Statuses.StatusId)))
            {
                var dateTimeOffset = GetRandomDateTimeOffset();
                var userId = Guid.NewGuid();

                var filler = new Filler<Models.Statuses.Status>();

                filler.Setup()
                    .OnProperty(status => status.Id).Use(statusId)
                    .OnProperty(status => status.Name).Use(statusId.GetDisplayName())
                    .OnType<DateTimeOffset>().Use(dateTimeOffset)
                    .OnProperty(status => status.CreatedByUserId).Use(userId)
                    .OnProperty(status => status.CreatedByUser).IgnoreIt()
                    .OnProperty(status => status.UpdatedByUserId).Use(userId)
                    .OnProperty(status => status.UpdatedByUser).IgnoreIt();

                statuses.Add(filler.Create());
            }

            return statuses;
        }


    }
}
