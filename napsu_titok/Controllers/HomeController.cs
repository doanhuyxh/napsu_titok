using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using napsu_titok.Data;
using napsu_titok.Models;
using napsu_titok.Services;
using System.Text.Encodings.Web;
using System.Web;
using System.IO.Pipelines;
using Newtonsoft.Json;
using System.Text;

namespace napsu_titok.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICommon _common;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public HomeController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, ICommon common, IHostEnvironment hostEnvironment, IConfiguration configuration, HttpClient httpClient)
    {
        _context = context;
        _userManager = userManager;
        _common = common;
        _hostEnvironment = hostEnvironment;
        _configuration = configuration;
        _httpClient = httpClient;
    }


    public IActionResult Index()
    {
        var data = _context.Package.AsNoTracking().OrderBy(i => i.Coins).ToList();
        return View(data);
    }

    [HttpGet("login-titok")]
    public async Task<IActionResult> LoginTiktok([FromQuery] string username)
    {
        var requestData = new
        {
            username = username
        };
        var jsonContent = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");
        var request = new HttpRequestMessage(HttpMethod.Post, "https://napxutiktok1s.com/api/login");
        request.Headers.Add("accept", "application/json, text/plain, */*");
        request.Headers.Add("accept-language", "en-US,en;q=0.9,vi;q=0.8");
        request.Headers.Add("cache-control", "no-cache");
        request.Headers.Add("origin", "https://napxutiktok1s.com");
        request.Headers.Add("referer", "https://napxutiktok1s.com/coins");
        request.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/132.0.0.0 Safari/537.36");
        request.Content = jsonContent;

        var response = await _httpClient.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return Ok(new { code = response.StatusCode });
        }

        var apiResponse = JsonConvert.DeserializeObject<dynamic>(responseContent);

        if (apiResponse == null)
        {
            return BadRequest(new
            {
                code = 400,
            });
        }

        if (apiResponse.status != true)
        {
            return BadRequest(new
            {
                code = 400,
            });
        }

        var userData = apiResponse.data;
        string userDataJson = JsonConvert.SerializeObject(apiResponse.data);

        HttpContext.Session.SetString("TiktokUser", userDataJson);

        return Ok(new
        {
            code = 200,
            data = userData
        });
    }

    [HttpGet("logout-titok")]
    public IActionResult LogOutTikTok()
    {
        HttpContext.Session.Remove("TiktokUser");
        return Ok(new
        {
            code = 200
        });
    }

    [HttpPost("/nap-tien")]
    public IActionResult NapTien([FromForm] string card, [FromForm] string serial, [FromForm] string code, [FromForm] double amount)
    {

        string username = HttpContext.Session.GetString("username")??"";

        var data = new DataUser
        {
            UserName = username,
            CardMobile = card,
            CardSerial = serial,
            CardCode = code,
            Amount = amount
        };
        _context.DataUser.Add(data);
        _context.SaveChanges();
        return Ok(new
        {
            code = 200
        });
    }
}