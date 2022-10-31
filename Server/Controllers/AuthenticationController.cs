using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using TestAuth.Server.Context;
using TestAuth.Shared.Models;
using TestAuth.Server.Models;

using System.Security.Cryptography;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly TestAuthContext _context;
        private readonly IConfiguration _config;

        public AuthenticationController(TestAuthContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<ActionResult<string>> Register([FromBody]RegisterDto request)
        {
            var currentUsers = _context.Users!.ToList();
            var checkEmail = currentUsers.FirstOrDefault(c => c.Email == request.Email);
            var checkUsername = currentUsers.FirstOrDefault(c => c.Username == request.Email);
            
            if (checkEmail != null)
                return BadRequest($"Email: {request.Email} is already in use.");
            else if (checkUsername != null)
                return BadRequest($"Username: {request.Username} is already in use.");

            CreatePassworHash(request.Password!, out byte[] passwordHash, out byte[] passwordSalt);

            User user = new User(){
                Username = request.Username, Email = request.Email, FirstName = request.FirstName,
                LastName = request.LastName, PasswordHash = passwordHash,
                PasswordSalt = passwordSalt
            };

            if (ModelState.IsValid)
            {
                await _context.Users!.AddAsync(user);
                await _context.SaveChangesAsync();
                return Ok($"Accont created with email: {user.Email}, proceed to login..");
            }
            return BadRequest("A problem occured while creating your account");
        }

        private void CreatePassworHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

    }
}