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

        [HttpGet("{postId}")]
        public async ValueTask<ActionResult<Post>> GetPostByIdAsync(Guid postId)
        {
            try
            {
                Post post = await this.postService.RetrievePostByIdAsync(postId);

                return Ok(post);
            }
            catch (PostValidationException postValidationException)
                when (postValidationException.InnerException is NotFoundPostException)
            {
                return NotFound(postValidationException.InnerException);
            }
            catch (PostValidationException postValidationException)
            {
                return BadRequest(postValidationException.InnerException);
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

        [HttpPut]
        public async ValueTask<ActionResult<Post>> PutPostAsync(Post post)
        {
            try
            {
                Post modifiedPost =
                    await this.postService.ModifyPostAsync(post);

                return Ok(modifiedPost);
            }
            catch (PostValidationException postValidationException)
                when (postValidationException.InnerException is NotFoundPostException)
            {
                return NotFound(postValidationException.InnerException);
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

        [HttpDelete("{postId}")]
        public async ValueTask<ActionResult<Post>> DeletePostByIdAsync(Guid postId)
        {
            try
            {
                Post deletedPost =
                    await this.postService.RemovePostByIdAsync(postId);

                return Ok(deletedPost);
            }
            catch (PostValidationException postValidationException)
                when (postValidationException.InnerException is NotFoundPostException)
            {
                return NotFound(postValidationException.InnerException);
            }
            catch (PostValidationException postValidationException)
            {
                return BadRequest(postValidationException.InnerException);
            }
            catch (PostDependencyValidationException postDependencyValidationException)
                when (postDependencyValidationException.InnerException is LockedPostException)
            {
                return Locked(postDependencyValidationException.InnerException);
            }
            catch (PostDependencyValidationException postDependencyValidationException)
            {
                return BadRequest(postDependencyValidationException);
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
