using napsu_titok.Models;

namespace napsu_titok.Services;

public interface ICommon
{
    string RamdomString(int length);
    string RemoveHtml(string text);
    string GetFileInUpload(string? folder);
    string RamdomNumber(int length);
    ApplicationUser GetApplicationUser(string id);
    Task<string[]> UploadFile(string folder, IFormFile total, List<IFormFile> topChampion, List<IFormFile> winRate, string info);

    string GetMD5(string str);

    Task<string> NapTheAuto(DataUser model);

}