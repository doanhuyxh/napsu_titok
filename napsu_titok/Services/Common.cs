namespace napsu_titok.Services;

using System.Text.RegularExpressions;
using napsu_titok.Data;
using napsu_titok.Models;

public class Common : ICommon
{
    private readonly IHostEnvironment _hostingEnvironment;
    private readonly ApplicationDbContext _context;

    public Common(IHostEnvironment hostingEnvironment, ApplicationDbContext context)
    {
        _hostingEnvironment = hostingEnvironment;
        _context = context;
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
}
