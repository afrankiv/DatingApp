using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
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

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            // ARCH: Repository Interface Access -> Returns ORM model, but var hides the typing
            var users = await _repo.GetUsers();
            // ARCH: Maps ORM Models into DTOs, but DTO is designed for the use case
            var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);

            return Ok(usersToReturn);
        }

        [HttpGet("{id}", Name="GetUser")]
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
    }
}