using Microsoft.AspNetCore.Mvc;
using napsu_titok.Data;
using napsu_titok.Services;

namespace napsu_titok.Controllers
{
    public class NapTheController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _iConfiguration;
        private readonly ICommon _common;
        private readonly IConfiguration _configuration;

        public NapTheController(ApplicationDbContext context, IConfiguration iConfiguration, ICommon common, IConfiguration configuration)
        {
            _context = context;
            _iConfiguration = iConfiguration;
            _common = common;
            _configuration = configuration;
        }

        [HttpGet("/check-card/call-back")]
        public IActionResult Index(string status, string message, string request_id, string declared_value, string card_value, string value, string amount, string code, string serial, string telco, string trans_id, string callback_sign)
        {
            var checkNapThe = _context.DataUser.FirstOrDefault(x => x.CardSerial == serial && x.CardCode == code);
            if (checkNapThe == null)
            {
                return new JsonResult(new
                {
                    code = 400,
                    status = "Lỗi",
                    message = "Không tìm thấy giao dịch"
                });
            }


            string messageMain = "";
            int codeMess = 0;
            switch (status)
            {
                case "1":
                    checkNapThe.Status = "Nạp tiền thành công";
                    messageMain = "Nạp tiền thành công";

                    codeMess = 200;
                    break;
                case "2":
                    checkNapThe.Status = "Thẻ thành công sai mệnh giá";
                    messageMain = "Nạp tiền sai mệnh giá";
                    codeMess = 200;
                    break;
                case "3":
                    checkNapThe.Status = "Thẻ lỗi";
                    messageMain = "Thẻ lỗi";
                    codeMess = 400;
                    break;
                case "4":
                    checkNapThe.Status = "Hệ thống bảo trì";
                    messageMain = "Hệ thống bảo trì";
                    codeMess = 400;
                    break;
                case "99":
                    checkNapThe.Status = "Thẻ chờ xử lý";
                    messageMain = "Thẻ chờ xử lý";
                    codeMess = 200;
                    break;
                case "100":
                    checkNapThe.Status = "Gửi thẻ thất bại";
                    messageMain = "Gửi thẻ thất bại";
                    codeMess = 400;
                    break;
                default:
                    checkNapThe.Status = "Trạng thái không xác định";
                    messageMain = "Trạng thái không xác định";
                    codeMess = 400;
                    break;
            }
            _context.SaveChanges();
            return new JsonResult(new
            {
                code = codeMess,
                status = codeMess == 200 ? "success" : "Lỗi",
                message = messageMain
            });

        }

    }
}
