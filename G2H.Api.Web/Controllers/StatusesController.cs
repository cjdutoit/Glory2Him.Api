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
using G2H.Api.Web.Models.Statuses.Exceptions;
using G2H.Api.Web.Services.Foundations.Statuses;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace G2H.Api.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusesController : RESTFulController
    {
        private readonly IStatusService statusService;

        public StatusesController(IStatusService statusService) =>
            this.statusService = statusService;

        [HttpGet]
        public ActionResult<IQueryable<Status>> GetAllStatuses()
        {
            try
            {
                IQueryable<Status> retrievedStatuses =
                    this.statusService.RetrieveAllStatuses();

                return Ok(retrievedStatuses);
            }
            catch (StatusDependencyException statusDependencyException)
            {
                return InternalServerError(statusDependencyException);
            }
            catch (StatusServiceException statusServiceException)
            {
                return InternalServerError(statusServiceException);
            }
        }

        [HttpGet("{statusId}")]
        public async ValueTask<ActionResult<Status>> GetStatusByIdAsync(StatusId statusId)
        {
            try
            {
                Status status = await this.statusService.RetrieveStatusByIdAsync(statusId);

                return Ok(status);
            }
            catch (StatusValidationException statusValidationException)
                when (statusValidationException.InnerException is NotFoundStatusException)
            {
                return NotFound(statusValidationException.InnerException);
            }
            catch (StatusValidationException statusValidationException)
            {
                return BadRequest(statusValidationException.InnerException);
            }
            catch (StatusDependencyException statusDependencyException)
            {
                return InternalServerError(statusDependencyException);
            }
            catch (StatusServiceException statuseserviceException)
            {
                return InternalServerError(statuseserviceException);
            }
        }
    }
}
