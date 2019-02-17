using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ApiCoreEcommerce.Dtos.Responses.Comments;
using ApiCoreEcommerce.Entities;
using ApiCoreEcommerce.Infrastructure.Extensions;
using ApiCoreEcommerce.Models;
using ApiCoreEcommerce.Services;
using ApiCoreEcommerce.Services.Interfaces;
using BlogDotNet.Errors;
using BlogDotNet.Models;
using BlogDotNet.Models.ViewModels.Requests.Comment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiCoreEcommerce.Controllers
{
    //[ApiController]
    [Route("api/")]
    public class CommentsController : Controller
    {
        private readonly IUsersService _usersService;

        private readonly ICommentsService _commentService;

        private readonly IAuthorizationService _authorizationService;
        private readonly IConfigurationService _configService;

        public CommentsController(IUsersService usersService, IAuthorizationService authorizationService,
            ICommentsService commentService,
            IConfigurationService configService)
        {
            _usersService = usersService;
            _authorizationService = authorizationService;
            _configService = configService;
            _commentService = commentService;
        }

        [HttpGet("products/{slug}/comments")]
        public async Task<IActionResult> GetComments(string slug, [FromQuery] int page = 1,
            [FromQuery] int pageSize = 5)
        {
            Tuple<int, List<Comment>> comments = await _commentService.FetchPageByProduct(slug);

            return StatusCodeAndDtoWrapper.BuildSuccess(
                CommentListDto.Build(comments.Item2, Request.Path, page, pageSize, comments.Item1)
            );
        }

        [HttpGet("products/{slug}/comments/{comment_id}")]
        [HttpGet("/comments/{comment_id}")]
        public async Task<IActionResult> GetDetails(string slug, long commentId, CreateOrEditCommentDto model)
        {
            Comment comment = await _commentService.FetchCommentByIdAsync(commentId);
            return StatusCodeAndDtoWrapper.BuildSuccess(CommentDetailsDto.Build(comment));
        }

        [Authorize]
        [HttpPost("products/{slug}/comments")]
        public async Task<IActionResult> Create(string slug, [FromBody] CreateOrEditCommentDto model)
        {
            if (!ModelState.IsValid)
                return StatusCodeAndDtoWrapper.BuilBadRequest(ModelState);

            long userId = Convert.ToInt64(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            ApplicationUser user = await _usersService.GetCurrentUserAsync();
            Comment comment = await _commentService.CreateAsync(user, slug, model, userId);

            return StatusCodeAndDtoWrapper.BuildSuccess(CommentDetailsDto.Build(comment));
        }

        
        [HttpPut]
        [Authorize]
        [Route("comments/{slug}/comments/{id}")]
        [Route("comments/{id}")]
        public async Task<IActionResult> Update([FromBody] CreateOrEditCommentDto dto, [FromRoute] long id, long slug)
        {
            // we need to load the user in order to let the Handler check it agains the owner if needed
            Comment comment = await _commentService.FetchCommentByIdAsync(id, includeUser: true);
            if (comment == null)
            {
                return StatusCodeAndDtoWrapper.BuildGenericNotFound();
            }

            var result = await _authorizationService.AuthorizeAsync(User, comment,
                _configService.GetDeleteCommentPolicyName());
            if (result.Succeeded)
            {
                int result2 = await _commentService.UpdateAsync(comment, dto);
                return StatusCodeAndDtoWrapper.BuildSuccess(CommentDetailsDto.Build(comment),
                    "Comment updated successfully");
                //return StatusCodeAndDtoWrapper.BuildSuccess("Comment updated successfully");
            }
            else
            {
                return StatusCodeAndDtoWrapper.BuildUnauthorized("Permission denied");
            }
        }
        
        [HttpDelete]
        [Authorize]
        [Route("comments/{slug}/comments/{id}")]
        [Route("comments/{id}")]
        public async Task<IActionResult> Delete([FromRoute] long id, string slug)
        {
            Comment comment = await _commentService.FetchCommentByIdAsync(id);
            if (comment == null)
            {
                return StatusCodeAndDtoWrapper.BuildGenericNotFound();
            }

            var result = await _authorizationService.AuthorizeAsync(User, comment,
                _configService.GetDeleteCommentPolicyName());
            if (result.Succeeded)
            {
                if ((await _commentService.DeleteAsync(id)) > 0)
                {
                    return StatusCodeAndDtoWrapper.BuildSuccess("Comment deleted successfully");
                }
                else
                {
                    return StatusCodeAndDtoWrapper.BuildErrorResponse("An error occured, try later");
                }
            }
            else
            {
                throw new PermissionDeniedException();
            }
        }
    }
}