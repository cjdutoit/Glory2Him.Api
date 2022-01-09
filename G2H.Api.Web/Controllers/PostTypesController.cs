// --------------------------------------------------------------------------------
// Copyright (c) Christo du Toit. All rights reserved.
// Licensed under the MIT License.
// See License.txt in the project root for license information.
// FREE TO USE TO HELP SHARE THE GOSPEL
// Mark 16:15 NIV "Go into all the world and preach the gospel to all creation."
// https://mark.bible/mark-16-15 
// --------------------------------------------------------------------------------

using System.Linq;
using System.Threading.Tasks;
using G2H.Api.Web.Models.PostTypes;
using G2H.Api.Web.Models.PostTypes.Exceptions;
using G2H.Api.Web.Services.Foundations.PostTypes;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace G2H.Api.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostTypesController : RESTFulController
    {
        private readonly IPostTypeService postTypeService;

        public PostTypesController(IPostTypeService postTypeService) =>
            this.postTypeService = postTypeService;

        [HttpGet]
        public ActionResult<IQueryable<PostType>> GetAllPostTypes()
        {
            try
            {
                IQueryable<PostType> retrievedPostTypes =
                    this.postTypeService.RetrieveAllPostTypes();

                return Ok(retrievedPostTypes);
            }
            catch (PostTypeDependencyException postTypeDependencyException)
            {
                return InternalServerError(postTypeDependencyException);
            }
            catch (PostTypeServiceException postTypeServiceException)
            {
                return InternalServerError(postTypeServiceException);
            }
        }

        [HttpGet("{postTypeId}")]
        public async ValueTask<ActionResult<PostType>> GetPostTypeByIdAsync(PostTypeId postTypeId)
        {
            try
            {
                PostType postType =
                    await this.postTypeService.RetrievePostTypeByIdAsync(postTypeId);

                return Ok(postType);
            }
            catch (PostTypeValidationException postTypeValidationException)
                when (postTypeValidationException.InnerException is NotFoundPostTypeException)
            {
                return NotFound(postTypeValidationException.InnerException);
            }
            catch (PostTypeValidationException postTypeValidationException)
            {
                return BadRequest(postTypeValidationException.InnerException);
            }
            catch (PostTypeDependencyException postTypeDependencyException)
            {
                return InternalServerError(postTypeDependencyException);
            }
            catch (PostTypeServiceException postTypeServiceException)
            {
                return InternalServerError(postTypeServiceException);
            }
        }
    }
}
