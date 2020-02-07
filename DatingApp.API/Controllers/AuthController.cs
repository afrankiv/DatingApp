using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public AuthController(IAuthRepository repo, IConfiguration config, IMapper mapper)
        {
            _mapper = mapper;
            _config = config;
            _repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            // Business Logic Access: Validates user through the repository
            if (await _repo.UserExists(userForRegisterDto.Username))
                return BadRequest("User already exists");

            // MAPPING from DTO to ORM model
            var userToCreate = _mapper.Map<User>(userForRegisterDto);

            // ARCH: Repository Interface Access -> Returns ORM model, but var hides the typing
            // Register new user in database
            var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);

            // MAPPING from ORM to DTO model
            var userToReturn = _mapper.Map<UserForDetailedDto>(createdUser);

            // Returns 201 Created Status + UserForDetailedDto + Location http://localhost:5000/api/Users/11
            return CreatedAtRoute("GetUser", new { controller = "Users", id = createdUser.Id }, userToReturn);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            // Repository Access
            var userFromRepo = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);

            if (userFromRepo == null)
                return Unauthorized();

            // Security token creation logic
            Claim[] claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_config.GetSection("AppSettings:Token").Value));

            // Microsoft.IdentityModel.Tokens.SymmetricSecurityKey 
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
            // Contains some information which used to create a security token.
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };
            // A Microsoft.IdentityModel.Tokens.SecurityTokenHandler designed for creating and validating Json Web Tokens. 
            SecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            // Microsoft.IdentityModel.Tokens.SecurityToken
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            var user = _mapper.Map<UserForListDto>(userFromRepo);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token),
                user
            });
        }
    }
}