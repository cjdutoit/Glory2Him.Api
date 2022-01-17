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
using G2H.Api.Web.Models.Posts;
using G2H.Api.Web.Models.Posts.Exceptions;
using G2H.Api.Web.Services.Foundations.Posts;
using Microsoft.AspNetCore.Mvc;
using RESTFulSense.Controllers;

namespace G2H.Api.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PostsController : RESTFulController
    {
        private readonly IPostService postService;

        public PostsController(IPostService postService) =>
            this.postService = postService;

        [HttpPost]
        public async ValueTask<ActionResult<Post>> PostPostAsync(Post post)
        {
            try
            {
                Post addedPost =
                    await this.postService.AddPostAsync(post);

                return Created(addedPost);
            }
            catch (PostValidationException postValidationException)
            {
                return BadRequest(postValidationException.InnerException);
            }
            catch (PostDependencyValidationException postValidationException)
                when (postValidationException.InnerException is InvalidPostReferenceException)
            {
                return FailedDependency(postValidationException.InnerException);
            }
            catch (PostDependencyValidationException postDependencyValidationException)
               when (postDependencyValidationException.InnerException is AlreadyExistsPostException)
            {
                return Conflict(postDependencyValidationException.InnerException);
            }
            catch (PostDependencyException postDependencyException)
            {
                return InternalServerError(postDependencyException);
            }
            catch (PostServiceException postServiceException)
            {
                return InternalServerError(postServiceException);
            }
        }

        [HttpGet]
        public ActionResult<IQueryable<Post>> GetAllPosts()
        {
            try
            {
                IQueryable<Post> retrievedPosts =
                    this.postService.RetrieveAllPosts();

                return Ok(retrievedPosts);
            }
            catch (PostDependencyException postDependencyException)
            {
                return InternalServerError(postDependencyException);
            }
            catch (PostServiceException postServiceException)
            {
                return InternalServerError(postServiceException);
            }
        }
    }
}
