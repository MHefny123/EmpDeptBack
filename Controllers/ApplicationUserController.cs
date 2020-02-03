using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using EmpDeptBack.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;


namespace EmpDeptBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationUserController : ControllerBase
    {
        #region CTOR & Definitions
        private UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ApplicationUserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        #endregion

        #region Insert Method
        [HttpPost]
        [Route("Login")]
        //Post:/api/ApplicationUser/Login
        public async Task<IActionResult> Login(LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.LoginUserName);

            if (model.LoginUserName == "Mustafa" && user == null)
            {
                await _userManager.CreateAsync(new ApplicationUser { UserName = "Mustafa", Email = "mustafa@gmail.com" }, "123456");
                user = await _userManager.FindByNameAsync(model.LoginUserName);

            }

            if (user != null && await _userManager.CheckPasswordAsync(user, model.LoginPassword))
            {
                // Get Role assigned to the user 
                var UserRoles = await _userManager.GetRolesAsync(user);
                if (UserRoles.Count == 0)
                {
                    await _roleManager.CreateAsync(new IdentityRole { Name = "Admin" });
                    await _userManager.AddToRoleAsync(user, "Admin");
                    UserRoles = await _userManager.GetRolesAsync(user);
                }

                IdentityOptions _options = new IdentityOptions();
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim("UserID",user.Id.ToString()),
                        new Claim(_options.ClaimsIdentity.RoleClaimType,UserRoles.FirstOrDefault())
                    }),
                    Expires = DateTime.UtcNow.AddDays(1),
                    SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes("1234567890123456")),
                    SecurityAlgorithms.HmacSha256Signature)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                var token = tokenHandler.WriteToken(securityToken);
                return Ok(new { token });
            }
            else
                return BadRequest("Username or Password is incorrect.");
        }
        #endregion


    }
}