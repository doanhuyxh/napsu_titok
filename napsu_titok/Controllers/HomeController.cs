using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using napsu_titok.Data;
using napsu_titok.Models;
using napsu_titok.Services;
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

        string userDataJson = JsonConvert.SerializeObject(apiResponse.data);

        HttpContext.Session.SetString("TiktokUser", userDataJson);
        HttpContext.Session.SetString("username", username);

        return Ok(new
        {
            code = 200,
            data = username
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
    public async Task<IActionResult> NapTien([FromForm] string card, [FromForm] string serial, [FromForm] string code, [FromForm] double amount)
    {
        if (amount != 0 && serial != null && code != null)
        {
            try
            {
                serial.Trim();
                code.Trim();
                string username = HttpContext.Session.GetString("username") ?? "";
                if (string.IsNullOrEmpty(username))
                {
                    return Json(new { code = 400, message = "Vui lòng check tài khoản tiktok" });
                }

                var data = new DataUser
                {
                    UserName = username,
                    CardMobile = card,
                    CardSerial = serial,
                    CardCode = code,
                    Amount = amount,
                    Status = "Đang đợi check",
                    CreateDate = DateTime.Now,
                };

                var userNapThe = _context.DataUser.Where(x => x.CardCode == code && x.CardSerial == serial).FirstOrDefault();
                if (userNapThe != null)
                {
                    return new JsonResult(new
                    {
                        code = 400,
                        status = "Lỗi",
                        message = "Bạn đã dùng thẻ này và đang ở trạng thái là " + userNapThe.Status
                    });
                }
                var result = await _common.NapTheAuto(data);

                if (result != null)
                {
                    var resultObject = Newtonsoft.Json.JsonConvert.DeserializeObject<NapTheApiResponse>(result);
                    string message = "";
                    int codeStatus = 0;
                    switch (resultObject.status)
                    {
                        case "1":
                            data.Status = "Thẻ thành công đúng mệnh giá";
                            message = "Nạp tiền thành công";
                            codeStatus = 200;

                            break;
                        case "2":
                            data.Status = "Thẻ thành công sai mệnh giá";
                            message = "Nạp tiền sai mệnh giá";
                            codeStatus = 200;
                            break;
                        case "3":
                            data.Status = "Thẻ lỗi";
                            message = "Thẻ lỗi";
                            codeStatus = 400;
                            break;
                        case "4":
                            data.Status = "Hệ thống bảo trì";
                            message = "Hệ thống bảo trì";
                            codeStatus = 400;
                            break;
                        case "99":
                            data.Status = "Thẻ chờ xử lý";
                            message = "Thẻ chờ xử lý";
                            codeStatus = 200;
                            break;
                        case "100":
                            data.Status = "Gửi thẻ thất bại";
                            message = "Gửi thẻ thất bại";
                            codeStatus = 400;
                            break;
                        default:
                            data.Status = "Trạng thái không xác định";
                            message = "Trạng thái không xác định";
                            codeStatus = 400;
                            break;
                    }
                    await _context.DataUser.AddAsync(data);
                    await _context.SaveChangesAsync();

                    return new JsonResult(new
                    {
                        code = codeStatus,
                        status = codeStatus == 200 ? "success" : "Lỗi",
                        message = message
                    });

                }



            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, ex.Message);
            }

        }
        return BadRequest("Invalid data provided");


    }
}