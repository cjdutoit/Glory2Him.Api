﻿// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using G2H.Api.Web.Models.Approvals;
using G2H.Api.Web.Models.Approvals.Exceptions;
using Moq;
using Xunit;

namespace G2H.Api.Web.Tests.Unit.Services.Foundations.Approvals
{
    public partial class ApprovalServiceTests
    {
        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfApprovalIsNullAndLogItAsync()
        {
            // given
            Approval nullApproval = null;
            var nullApprovalException = new NullApprovalException();

            var expectedApprovalValidationException =
                new ApprovalValidationException(nullApprovalException);

            // when
            ValueTask<Approval> modifyApprovalTask =
                this.approvalService.ModifyApprovalAsync(nullApproval);

            // then
            await Assert.ThrowsAsync<ApprovalValidationException>(() =>
                modifyApprovalTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedApprovalValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateApprovalAsync(It.IsAny<Approval>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfApprovalIsInvalidAndLogItAsync()
        {
            // given 
            var invalidApproval = new Approval();

            var invalidApprovalException = new InvalidApprovalException();

            invalidApprovalException.AddData(
                key: nameof(Approval.Id),
                values: "Id is required");

            invalidApprovalException.AddData(
                key: nameof(Approval.StatusId),
                values: "Id is required");

            invalidApprovalException.AddData(
                key: nameof(Approval.CreatedDate),
                values: "Date is required");

            invalidApprovalException.AddData(
                key: nameof(Approval.CreatedByUserId),
                values: "Id is required");

            invalidApprovalException.AddData(
                key: nameof(Approval.UpdatedDate),
                values: "Date is required");

            invalidApprovalException.AddData(
                key: nameof(Approval.UpdatedByUserId),
                values: "Id is required");

            var expectedApprovalValidationException =
                new ApprovalValidationException(invalidApprovalException);

            // when
            ValueTask<Approval> modifyApprovalTask =
                this.approvalService.ModifyApprovalAsync(invalidApproval);

            //then
            await Assert.ThrowsAsync<ApprovalValidationException>(() =>
                modifyApprovalTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedApprovalValidationException))),
                        Times.Once());

            this.storageBrokerMock.Verify(broker =>
                broker.UpdateApprovalAsync(It.IsAny<Approval>()),
                    Times.Never);

            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task ShouldThrowValidationExceptionOnModifyIfUpdatedDateIsSameAsCreatedDateAndLogItAsync()
        {
            // given
            DateTimeOffset randomDateTimeOffset = GetRandomDateTimeOffset();
            Approval randomApproval = CreateRandomApproval(randomDateTimeOffset);
            Approval invalidApproval = randomApproval;
            var invalidApprovalException = new InvalidApprovalException();

            invalidApprovalException.AddData(
                key: nameof(Approval.UpdatedDate),
                values: $"Date is the same as {nameof(Approval.CreatedDate)}");

            var expectedApprovalValidationException =
                new ApprovalValidationException(invalidApprovalException);

            // when
            ValueTask<Approval> modifyApprovalTask =
                this.approvalService.ModifyApprovalAsync(invalidApproval);

            // then
            await Assert.ThrowsAsync<ApprovalValidationException>(() =>
                modifyApprovalTask.AsTask());

            this.loggingBrokerMock.Verify(broker =>
                broker.LogError(It.Is(SameExceptionAs(
                    expectedApprovalValidationException))),
                        Times.Once);

            this.storageBrokerMock.Verify(broker =>
                broker.SelectApprovalByIdAsync(invalidApproval.Id),
                    Times.Never);

            this.dateTimeBrokerMock.VerifyNoOtherCalls();
            this.loggingBrokerMock.VerifyNoOtherCalls();
            this.storageBrokerMock.VerifyNoOtherCalls();
        }
    }
}
