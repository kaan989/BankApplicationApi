using BankApplication.IRepository;
using BankApplication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BankApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppUserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IAppUserRepository _userRepository;
        public AppUserController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IConfiguration configuration, IAppUserRepository appUserRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _userRepository = appUserRepository;

        }



        private string GenerateJwtToken(AppUser user, IList<string> roles)
        {

            // Claims (talep) listesi oluşturuluyor
            var claims = new List<Claim>

    {
        new Claim(JwtRegisteredClaimNames.Sub, user.UserName), // Kullanıcı adı
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Token ID
        new Claim(JwtRegisteredClaimNames.Email, user.Email) // E-posta
    };

            // Rolleri claim olarak ekliyoruz
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Anahtar ve imzalama işlemi
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Token oluşturuluyor
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"])), // Süre
                signingCredentials: creds);

            // Token'ı döndür
            return new JwtSecurityTokenHandler().WriteToken(token);
        }







        private string GenerateJwtTokenRegister(AppUser user)
        {
            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            var roles = _userManager.GetRolesAsync(user).Result;
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireMinutes"])),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }



        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = new AppUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                IdNumber = model.IdNumber,
                Address = model.Address,    
                UserName = model.Username,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Kullanıcıyı varsayılan olarak 'user' rolüne atıyoruz
                await _userManager.AddToRoleAsync(user, "user");

                // JWT token oluşturma işlemi
                var token = GenerateJwtTokenRegister(user);

                return Ok(new { message = "User registered successfully!", token });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }

            return BadRequest(ModelState);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null)
            {
                var passwordCheck = await _userManager.CheckPasswordAsync(user, model.Password);
                if (passwordCheck)
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
                    if (result.Succeeded)
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        var token = GenerateJwtToken(user, roles);
                        var userResponse = new
                        {
                            Token = token,
                            Id = user.Id,
                            Username = user.UserName,
                            Email = user.Email,
                            Roles = await _userManager.GetRolesAsync(user)
                        };
                        return Ok(userResponse);
                    }
                }
            }

            return Unauthorized(new { message = "Invalid login attempt." });
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = _userRepository.GetAll();
            return Ok(users);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(string id)
        {
            var deletedUser = _userRepository.GetById(id);
            if(deletedUser == null)
            {
                return NotFound();
            }

            if (!_userRepository.DeleteProfile(deletedUser))
            {
                return StatusCode(500, "something went wrong while deleting");
            }

            return Ok();
        }

        [HttpGet("{id}")]
        public IActionResult GetUserById(string id)
        {
           var user = _userRepository.GetById(id);
            if(user == null)
                return NotFound();
            return Ok(user);
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            _signInManager.SignOutAsync();
            return Ok(new { message = "User logged out successfully!" });
        }
    }
}
