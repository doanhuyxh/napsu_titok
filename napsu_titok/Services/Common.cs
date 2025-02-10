namespace napsu_titok.Services;

using Microsoft.Extensions.Configuration;
using napsu_titok.Data;
using napsu_titok.Models;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

public class Common : ICommon
{
    private readonly IHostEnvironment _hostingEnvironment;
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public Common(IHostEnvironment hostingEnvironment, ApplicationDbContext context, IConfiguration configuration, HttpClient httpClient)
    {
        _hostingEnvironment = hostingEnvironment;
        _context = context;
        _configuration = configuration;
        _httpClient = httpClient;
    }

    public ApplicationUser GetApplicationUser(string id)
    {
        var user = _context.ApplicationUser.FirstOrDefault(i => i.Id == id);
        return user;
    }

    public string GetFileInUpload(string? folder)
    {
        try
        {
            if (string.IsNullOrEmpty(folder))
            {
                return "";
            }

            string anhketquaPath = Path.Combine(_hostingEnvironment.ContentRootPath, "wwwroot", "Upload", folder, "anhketqua");
            string file_url = "";
            var filesSuccess = Directory.GetFiles(anhketquaPath).Select(Path.GetFileName).ToList();

            file_url = $"/Upload/{folder}/anhketqua/{filesSuccess.First()}";

            return file_url;
        }
        catch
        {
            return "";
        }

    }

    public string RamdomNumber(int length)
    {
        var random = new Random();
        const string chars = "123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public string RamdomString(int length)
    {
        var random = new Random();
        const string chars = "abcdefghijklmnopqrstuvwxyz123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public string RemoveHtml(string text)
    {
        if (string.IsNullOrEmpty(text))
            return string.Empty;

        return Regex.Replace(text, "<.*?>", string.Empty);
    }

    public async Task<string[]> UploadFile(string folder, IFormFile total, List<IFormFile> topChampion, List<IFormFile> winRate, string info)
    {
        string[] result = new string[5];

        string rootPath = Path.Combine(_hostingEnvironment.ContentRootPath, "wwwroot", "Upload");

        string orderPath = Path.Combine(rootPath, folder);


        // Các thư mục con cần tạo
        string anhtongPath = Path.Combine(orderPath, "anhtong");
        string anhtilethangPath = Path.Combine(orderPath, "anhtilethang");
        string anhtopPath = Path.Combine(orderPath, "anhtop");
        string anhketquaPath = Path.Combine(orderPath, "anhketqua");
        string errorsPath = Path.Combine(orderPath, "errors");
        string infoTxtPath = Path.Combine(orderPath, "info.txt");

        // Tạo thư mục nếu chưa tồn tại
        Directory.CreateDirectory(rootPath);
        Directory.CreateDirectory(anhtongPath);
        Directory.CreateDirectory(anhtilethangPath);
        Directory.CreateDirectory(anhtopPath);
        Directory.CreateDirectory(anhketquaPath);
        Directory.CreateDirectory(errorsPath);

        // Lưu các tệp tin vào đúng thư mục
        if (total != null)
        {
            string totalFilePath = Path.Combine(anhtongPath, total.FileName);
            await SaveFile(total, totalFilePath);
        }

        if (topChampion.Count > 0)
        {
            for (int i = 0; i < topChampion.Count; i++)
            {
                string topChampionFilePath = Path.Combine(anhtopPath, topChampion[i].FileName);
                await SaveFile(topChampion[i], topChampionFilePath);
            }
        }

        if (winRate.Count > 0)
        {
            for (int i = 0; i < winRate.Count; i++)
            {
                string winRateFilePath = Path.Combine(anhtilethangPath, winRate[i].FileName);
                await SaveFile(winRate[i], winRateFilePath);
            }
        }

        // Tạo file info.txt nếu chưa có
        if (!File.Exists(infoTxtPath))
        {
            await File.WriteAllTextAsync(infoTxtPath, info);
        }

        return result;
    }

    private async Task<string> SaveFile(IFormFile file, string path)
    {

        if (file == null)
        {
            return "";
        }

        using (var stream = new FileStream(path, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return path;
    }
    public string GetMD5(string str)
    {
        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] fromData = Encoding.UTF8.GetBytes(str);
        byte[] targetData = md5.ComputeHash(fromData);
        string byte2String = null;

        for (int i = 0; i < targetData.Length; i++)
        {
            byte2String += targetData[i].ToString("x2");

        }
        return byte2String;
    }

    public async Task<string> NapTheAuto(DataUser model)
    {
        var partnerKey = _configuration.GetSection("PartnerKey").Value;
        var partnerId = _configuration.GetSection("PartnerId").Value;
        var signMd5 = partnerKey + model.CardCode + model.CardSerial;
        var hashSign = GetMD5(signMd5);
        var random = new Random();
        var requestId = random.Next(100000, 1000000000).ToString();
        var requestData = new
        {
            telco = model.CardMobile,
            partner_id = partnerId,
            request_id = requestId,
            serial = model.CardSerial,
            code = model.CardCode,
            amount = model.Amount,
            sign = hashSign,
            command = "charging"
        };

        var url = "https://thesieure.com/chargingws/v2";
        var json = Newtonsoft.Json.JsonConvert.SerializeObject(requestData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }
        else
        {
            var errorMessage = $"Error: {response.StatusCode}";
            throw new HttpRequestException(errorMessage);
        }
    }
}
