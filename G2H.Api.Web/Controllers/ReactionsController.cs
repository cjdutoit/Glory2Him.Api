// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System.Linq;
using G2H.Api.Web.Models.Reactions;
using G2H.Api.Web.Models.Reactions.Exceptions;
using G2H.Api.Web.Services.Foundations.Reactions;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace G2H.Api.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReactionsController : RESTFulController
    {
        private readonly IReactionService reactionService;

        public ReactionsController(IReactionService reactionService) =>
            this.reactionService = reactionService;

        [HttpGet]
        public ActionResult<IQueryable<Reaction>> GetAllReactions()
        {
            try
            {
                IQueryable<Reaction> retrievedReactions =
                    this.reactionService.RetrieveAllReactions();

                return Ok(retrievedReactions);
            }
            catch (ReactionDependencyException reactionDependencyException)
            {
                return InternalServerError(reactionDependencyException);
            }
            catch (ReactionServiceException reactionServiceException)
            {
                return InternalServerError(reactionServiceException);
            }
        }
    }
}
