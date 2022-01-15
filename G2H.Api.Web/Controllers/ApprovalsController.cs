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
using G2H.Api.Web.Models.Approvals;
using G2H.Api.Web.Models.Approvals.Exceptions;
using G2H.Api.Web.Services.Foundations.Approvals;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace G2H.Api.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApprovalsController : RESTFulController
    {
        private readonly IApprovalService approvalService;

        public ApprovalsController(IApprovalService approvalService) =>
            this.approvalService = approvalService;

        [HttpPost]
        public async ValueTask<ActionResult<Approval>> PostApprovalAsync(Approval approval)
        {
            try
            {
                Approval addedApproval =
                    await this.approvalService.AddApprovalAsync(approval);

                return Created(addedApproval);
            }
            catch (ApprovalValidationException approvalValidationException)
            {
                return BadRequest(approvalValidationException.InnerException);
            }
            catch (ApprovalDependencyValidationException approvalValidationException)
                when (approvalValidationException.InnerException is InvalidApprovalReferenceException)
            {
                return FailedDependency(approvalValidationException.InnerException);
            }
            catch (ApprovalDependencyValidationException approvalDependencyValidationException)
               when (approvalDependencyValidationException.InnerException is AlreadyExistsApprovalException)
            {
                return Conflict(approvalDependencyValidationException.InnerException);
            }
            catch (ApprovalDependencyException approvalDependencyException)
            {
                return InternalServerError(approvalDependencyException);
            }
            catch (ApprovalServiceException approvalServiceException)
            {
                return InternalServerError(approvalServiceException);
            }
        }

        [HttpGet]
        public ActionResult<IQueryable<Approval>> GetAllApprovals()
        {
            try
            {
                IQueryable<Approval> retrievedApprovals =
                    this.approvalService.RetrieveAllApprovals();

                return Ok(retrievedApprovals);
            }
            catch (ApprovalDependencyException approvalDependencyException)
            {
                return InternalServerError(approvalDependencyException);
            }
            catch (ApprovalServiceException approvalServiceException)
            {
                return InternalServerError(approvalServiceException);
            }
        }

        [HttpGet("{approvalId}")]
        public async ValueTask<ActionResult<Approval>> GetApprovalByIdAsync(Guid approvalId)
        {
            try
            {
                Approval approval = await this.approvalService.RetrieveApprovalByIdAsync(approvalId);

                return Ok(approval);
            }
            catch (ApprovalValidationException approvalValidationException)
                when (approvalValidationException.InnerException is NotFoundApprovalException)
            {
                return NotFound(approvalValidationException.InnerException);
            }
            catch (ApprovalValidationException approvalValidationException)
            {
                return BadRequest(approvalValidationException.InnerException);
            }
            catch (ApprovalDependencyException approvalDependencyException)
            {
                return InternalServerError(approvalDependencyException);
            }
            catch (ApprovalServiceException approvalServiceException)
            {
                return InternalServerError(approvalServiceException);
            }
        }

        [HttpPut]
        public async ValueTask<ActionResult<Approval>> PutApprovalAsync(Approval approval)
        {
            try
            {
                Approval modifiedApproval =
                    await this.approvalService.ModifyApprovalAsync(approval);

                return Ok(modifiedApproval);
            }
            catch (ApprovalValidationException approvalValidationException)
                when (approvalValidationException.InnerException is NotFoundApprovalException)
            {
                return NotFound(approvalValidationException.InnerException);
            }
            catch (ApprovalValidationException approvalValidationException)
            {
                return BadRequest(approvalValidationException.InnerException);
            }
            catch (ApprovalDependencyValidationException approvalValidationException)
                when (approvalValidationException.InnerException is InvalidApprovalReferenceException)
            {
                return FailedDependency(approvalValidationException.InnerException);
            }
            catch (ApprovalDependencyValidationException approvalDependencyValidationException)
               when (approvalDependencyValidationException.InnerException is AlreadyExistsApprovalException)
            {
                return Conflict(approvalDependencyValidationException.InnerException);
            }
            catch (ApprovalDependencyException approvalDependencyException)
            {
                return InternalServerError(approvalDependencyException);
            }
            catch (ApprovalServiceException approvalServiceException)
            {
                return InternalServerError(approvalServiceException);
            }
        }

        [HttpDelete("{approvalId}")]
        public async ValueTask<ActionResult<Approval>> DeleteApprovalByIdAsync(Guid approvalId)
        {
            try
            {
                Approval deletedApproval =
                    await this.approvalService.RemoveApprovalByIdAsync(approvalId);

                return Ok(deletedApproval);
            }
            catch (ApprovalValidationException approvalValidationException)
                when (approvalValidationException.InnerException is NotFoundApprovalException)
            {
                return NotFound(approvalValidationException.InnerException);
            }
            catch (ApprovalValidationException approvalValidationException)
            {
                return BadRequest(approvalValidationException.InnerException);
            }
            catch (ApprovalDependencyValidationException approvalDependencyValidationException)
                when (approvalDependencyValidationException.InnerException is LockedApprovalException)
            {
                return Locked(approvalDependencyValidationException.InnerException);
            }
            catch (ApprovalDependencyValidationException approvalDependencyValidationException)
            {
                return BadRequest(approvalDependencyValidationException);
            }
            catch (ApprovalDependencyException approvalDependencyException)
            {
                return InternalServerError(approvalDependencyException);
            }
            catch (ApprovalServiceException approvalServiceException)
            {
                return InternalServerError(approvalServiceException);
            }
        }
    }
}
