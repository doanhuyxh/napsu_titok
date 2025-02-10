using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using napsu_titok.Data;
using napsu_titok.Models;
using napsu_titok.Services;
using Microsoft.EntityFrameworkCore;

namespace napsu_titok.Controllers;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IConfiguration _iConfiguration;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICommon _common;
    private readonly IConfiguration _configuration;

    public AccountController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, SignInManager<ApplicationUser> signInManager, IConfiguration iConfiguration, ICommon common, IConfiguration configuration)
    {
        _context = context;
        _signInManager = signInManager;
        _iConfiguration = iConfiguration;
        _userManager = userManager;
        _common = common;
        _configuration = configuration;
    }

    [HttpGet("login")]
    public IActionResult Login() { return View(); }

    [HttpGet("register")]
    public IActionResult Register() { return View(); }

    [HttpGet("access-denied")]
    public IActionResult AccessDenied() { return View(); }


    [HttpPost("login")]
    public async Task<IActionResult> LoginApi([FromForm] string email, [FromForm] string password)
    {
        var user = await _userManager.FindByEmailAsync(email);

        if (user == null)
        {
            return Ok(new
            {
                code = 404,
                message = "Tài khoản mật khẩu không đúng"
            });
        }

        var result = await _signInManager.PasswordSignInAsync(user.UserName ?? "", password, true, lockoutOnFailure: false);

        if (!result.Succeeded)
        {
            return Ok(new
            {
                code = 404,
                message = "Tài khoản mật khẩu không đúng"
            });
        }

        var role = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, user.UserName??""),
                new Claim(ClaimTypes.Email, user.Email??""),
                new Claim(ClaimTypes.Role, role.FirstOrDefault()!),
                };

        var claimsIdentity = new ClaimsIdentity(
            claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1),
        };


        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);


        if (role.Contains("AdminWeb"))
        {
            return Ok(new
            {
                code = 200,
                message = "",
                url = "/admin-dashboard"
            });
        }
        else
        {
            return Ok(new
            {
                code = 200,
                message = "",
                url = "/"
            });
        }


    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterApi([FromForm] string email, [FromForm] string password)
    {
        try
        {

         

            ApplicationUser user = new ApplicationUser
            {
                Email = email,
                UserName = email.Replace("@gmail.com", ""),
            };
            var result = await _userManager.CreateAsync(user, password);


            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Member");
                await _signInManager.SignInAsync(user, isPersistent: false);
                return Ok(new
                {
                    code = 200,
                    message = "",
                    url = "/"
                });
            }
            else
            {
                return Ok(new
                {
                    code = 400,
                    message = "Tạo tài khoản thất bại",
                    
                });
            }
        }
        catch
        {
            return Ok(new
            {
                code = 400,
                message = "Tạo tài khoản thất bại",

            });
        }
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        await _signInManager.SignOutAsync();
        return Redirect("/login");
    }

}