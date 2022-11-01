using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using TestAuth.Server.Context;
using TestAuth.Shared.Models;
using TestAuth.Server.Models;

using System.Security.Cryptography;
using System.Security.Claims;
// using System.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

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

        [HttpGet("users")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<User[]>> GetUsers()
        {
            var users = await _context.Users!.ToListAsync();
            return Ok(users);
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

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginDto request)
        {
            if (request.Email == null && request.Password == null)
                return BadRequest("Both fields are required. Please try again.");

            var user = await _context.Users!.FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
                return BadRequest($"User with email: {request.Email}, was not found.");
            
            if (!VerifyPasswordHash(request.Password!, user.PasswordHash!, user.PasswordSalt!))
                return BadRequest("Wrong password");

            var token = CreateToken(user);
            var refreshToken = GenerateRefreshToken();
            SetRefreshToken(user, refreshToken);

            return Ok(token);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<string>> RefreshToken([FromQuery]string username)
        {
            var user = await _context.Users!.FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return NotFound($"User {username} was not found");

            var refreshToken = Request.Cookies["refreshToken"];

            if (!user.RefreshToken!.Equals(refreshToken))
                return Unauthorized("Invalid request token");
            else if (user.TokenExpires < DateTime.Now)
                return Unauthorized("Token Expired");

            string token = CreateToken(user);
            var newRefreshToken = GenerateRefreshToken();
            SetRefreshToken(user, newRefreshToken);

            return Ok(token);
        }

        private void CreatePassworHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        
        private bool VerifyPasswordHash(string password, [FromQuery]byte[] PasswordHash, [FromQuery]byte[] PasswordSalt)
        {
            using (var hmac = new HMACSHA512(PasswordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(PasswordHash);
            }
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username!),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(ClaimTypes.Email, user.Email!)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _config.GetSection("Jwt:Key").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            
            return jwt;
        }

        private JWT GenerateRefreshToken()
        {
            var refreshToken = new JWT
            {
                Key = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Issuer = _config.GetSection("Jwt:Issuer").Value,
                Audience = _config.GetSection("Jwt:Audience").Value,
                Expires = DateTime.UtcNow.AddDays(1)
            };
            // save token
            _context.Tokens!.Add(refreshToken);
            _context.SaveChanges();
            
            return refreshToken;
        }

        private void SetRefreshToken(User user, JWT refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = refreshToken.Expires
            };

            Response.Cookies.Append("refreshToken", refreshToken.Key!, cookieOptions);

            user.RefreshToken = refreshToken.Key;
            user.TokenExpires = refreshToken.Expires;
            _context.SaveChanges();
        }
    }
}