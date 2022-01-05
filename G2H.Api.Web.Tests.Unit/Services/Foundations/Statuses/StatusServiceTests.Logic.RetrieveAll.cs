// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System.Linq;
using FluentAssertions;
using G2H.Api.Web.Models.Statuses;
using Moq;
using Xunit;

namespace G2H.Api.Web.Tests.Unit.Services.Foundations.Statuses
{
    public partial class StatusServiceTests
    {
        [Fact]
        public void ShouldReturnStatuses()
        {
            // given
            IQueryable<Status> randomStatuses = CreateRandomStatuses();
            IQueryable<Status> storageStatuses = randomStatuses;
            IQueryable<Status> expectedStatuses = storageStatuses;

            this.storageBrokerMock.Setup(broker =>
                broker.SelectAllStatuses())
                    .Returns(storageStatuses);

            // when
            IQueryable<Status> actualStatuses =
                this.statusService.RetrieveAllStatuses();

            // then
            actualStatuses.Should().BeEquivalentTo(expectedStatuses);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectAllStatuses(),
                    Times.Once);

            this.storageBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
        }
    }
}
