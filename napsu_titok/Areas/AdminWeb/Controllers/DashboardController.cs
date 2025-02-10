using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using napsu_titok.Data;
using napsu_titok.Models;

namespace napsu_titok.Areas.AdminWeb.Controllers;

[Area("AdminWeb")]
[Authorize(Roles = "AdminWeb")]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("admin-dashboard")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("admin-package")]
    public IActionResult Package()
    {
        var data = _context.Package.AsNoTracking().OrderBy(i => i.Id).ToList();
        return View(data);
    }

    [HttpGet("admin-package-delete")]
    public IActionResult DeletePackage([FromQuery] int id)
    {
        var package = _context.Package.FirstOrDefault(i => i.Id == id);
        if (package != null)
        {
            _context.Remove(package);
            _context.SaveChanges();
        }

        return Ok(new
        {
            code = 200
        });
    }

    [HttpPost("admin-package-save")]
    public IActionResult PackageSave([FromForm] double Amount, [FromForm] double Coins, [FromForm] double Promotions, [FromForm] int id = 0)
    {
        var package = new Package();

        if(id != 0)
        {
            package = _context.Package.FirstOrDefault(i => i.Id == id);
            package.Amount = Amount;
            package.Coins = Coins;
            package.Promotions = Promotions;

            _context.Update(package);
            _context.SaveChanges();

        }
        else
        {
            package.Amount = Amount;
            package.Coins = Coins;
            package.Promotions = Promotions;

            _context.Add(package);
            _context.SaveChanges();
        }

        return Ok(new
        {
            code = 200
        });
    }


    [HttpGet("admin-transaction")]
    public IActionResult Transaction([FromQuery] int page =1, [FromQuery] int pageSize = 30)
    {
        if (page < 1)
        {
            page = 1;
        }
        ViewBag.page = page;
        var data = _context.DataUser.AsNoTracking().OrderByDescending(i => i.Id).Skip((page-1)*pageSize).Take(pageSize).ToList();
        return View(data);
    }




}