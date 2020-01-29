using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _repo;

        private readonly IMapper _mapper;

        public UsersController(IDatingRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        /// <summary>
        /// GET: Returns users collection and pagination headers, based on query parameters
        /// </summary>
        /// <param name="userParams">Request parameters passesd as query string</param>
        /// <returns>Users collection and response headers with pagination info</returns>
        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        {
            // System.Security.Claims.ClaimTypes
            // ClaimsPrincipal ControllerBase.User -> Gets the System.Security.Claims.ClaimsPrincipal for user associated with the executing action.
            // Logged in User
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var userFromRepo = await _repo.GetUser(currentUserId);
            // Attach user id to user params on the server side
            userParams.UserId = currentUserId;

            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = userFromRepo.Gender == "male" ? "female" : "male";
            }

            // ARCH: Repository Interface Access -> Returns custom PagedList collection
            var users = await _repo.GetUsers(userParams);

            // ARCH: Maps ORM Models into DTOs, but DTO is designed for the use case
            var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);

            // ARCH: Add additional metadata(pagination) about users collection into HTTP Response Headers
            Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            // Microsoft.AspNetCore.Mvc(WebApi):ControllerBase:
            // Returns serialized collection in response with status code 200:OK
            return Ok(usersToReturn);
        }

        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            // ARCH: Repository Interface Access -> Returns ORM model, but var hides the typing
            var user = await _repo.GetUser(id);

            // ARCH: Maps ORM Models into DTOs, but DTO is designed for the use case
            var userToReturn = _mapper.Map<UserForDetailedDto>(user);

            return Ok(userToReturn);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            // ARCH: Repository Interface Access -> Returns ORM model, but var hides the typing
            var userFromRepo = await _repo.GetUser(id);
            // ARCH: Maps ORM Models into DTOs, but DTO is designed for the use case
            _mapper.Map(userForUpdateDto, userFromRepo);

            // ARCH: Repository Interface Access
            if (await _repo.SaveAll())
                return NoContent();

            throw new Exception($"Updating user {id} failed on save");
        }

        [HttpPost("{id}/like/{recipientId}")]
        public async Task<IActionResult> LikeUser(int id, int recipientId)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var like = await _repo.GetLike(id, recipientId);
            if (like != null)
                return BadRequest("You already liked this user.");

            if (await _repo.GetUser(recipientId) == null)
                return NotFound();

            like = new Like
            {
                LikerId = id,
                LikeeId = recipientId
            };

            _repo.Add<Like>(like);

            if (await _repo.SaveAll())
                return Ok();
            
            return BadRequest("Failed to like user");
        }
    }
}