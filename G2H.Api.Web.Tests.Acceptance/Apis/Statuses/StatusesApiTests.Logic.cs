﻿// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using G2H.Api.Web.Tests.Acceptance.Models.Statuses;
using Xunit;

namespace G2H.Api.Web.Tests.Acceptance.Apis.Statuses
{
    public partial class StatusesApiTests
    {
        [Fact]
        public async Task ShouldGetAllStatusesAsync()
        {
            // given
            List<Status> statuses = await CreateStatusesAsync();

            List<Status> expectedStatuses = statuses;

            // when
            List<Status> actualStatuses = await this.apiBroker.GetAllStatusesAsync();

            // then
            foreach (Status expectedStatus in expectedStatuses)
            {
                Status actualStatus = actualStatuses.Single(status => status.Id == expectedStatus.Id);
                actualStatus.Should().BeEquivalentTo(expectedStatus);
            }
        }
    }
}
