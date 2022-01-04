// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System.Threading.Tasks;
using G2H.Api.Web.Models.Statuses;
using G2H.Api.Web.Models.Statuses.Exceptions;
using Moq;
using Xunit;

namespace G2H.Api.Web.Tests.Unit.Services.Foundations.Statuses
{
    public partial class StatusServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnAddIfStatusIsNullAndLogItAsync()
        {
            // given
            Status nullStatus = null;

            var nullStatusException =
                new NullStatusException();

            var expectedStatusValidationException =
                new StatusValidationException(nullStatusException);

            // when
            ValueTask<Status> addStatusTask =
                this.statusService.AddStatusAsync(nullStatus);

            // then
            await Assert.ThrowsAsync<StatusValidationException>(() =>
                addStatusTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedStatusValidationException))),
                        Times.Once);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
