using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AppUI.Data;
using AppUI.Models.Entities;
using AppUI.Models.CustomIdentity;
using Microsoft.AspNetCore.Identity;
using AppUI.Areas.Admin.Models.ViewModels;
using AppUI.Areas.Admin.Models.DTOs;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using System.Drawing;

namespace AppUI.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = "Administrator")]

public class SaleController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    public SaleController(ApplicationDbContext context, IMapper mapper, UserManager<AppUser> userManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
    }

    [Route("Admin/Sales")]
    public async Task<IActionResult> Index()
    {
        var orders = await _context.Orders
            .Include(c => c.OrderContents)
            .Include(c => c.Customer)
            .Where(c => c.IsFinal)
            .ToListAsync();

        var ordercontents = _mapper.Map<List<OrderContentVm>>(
            await _context.OrderContents
            .Include(c => c.Order)
            .Include(c => c.Order!.Customer)
            .Include(c => c.VideoGame)
            .Include(c => c.VideoGame!.Offer)
            .ToListAsync());

        ordercontents = ordercontents
            .Where(c => orders.Any(j => j.Id == c.OrderId))
            .ToList();

        var query = ordercontents.GroupBy(name => new { name.VideoGameId, name.VideoGame!.Title })
            .Select(sale => new
            {
                VideoGameId = sale.Key.VideoGameId,
                Title = sale.Key.Title,
                Quantity = sale.Sum(name => name.Quantity),
                TotalSales = sale.Sum(name => name.TotalPrice)
            });

        var sales = new List<SaleVm> ();

        foreach (var item in query)
        {
            sales.Add(new SaleVm()
            {
                VideoGameId = item.VideoGameId,
                Title = item.Title,
                Quantity = item.Quantity,
                TotalSales = item.TotalSales
            });
        }
        
        var output = new IndexSalesVm()
        {
            ListSales = sales
        };
        return View(output);
    }

    // GET: Sale/DetailsItem/5
    [Route("Admin/Sale/DetailsItem")]
    public async Task<IActionResult> DetailsItem(int? id)
    {
        var orders = await _context.Orders
            .Include(c => c.OrderContents)
            .Include(c => c.Customer)
            .Where(c => c.IsFinal)
            .ToListAsync();

        var ordercontents = _mapper.Map<List<OrderContentVm>>(
            await _context.OrderContents
            .Include(c => c.Order)
            .Include(c => c.Order!.Customer)
            .Include(c => c.VideoGame)
            .Include(c => c.VideoGame!.Offer)
            .Where(c => c.VideoGameId == id)
            .ToListAsync());

        ordercontents = ordercontents
            .Where(c => orders.Any(j => j.Id == c.OrderId))
            .ToList();

        var query = ordercontents.GroupBy(name => new { name.VideoGameId, name.VideoGame!.Title })
            .Select(sale => new
            {
                VideoGameId = sale.Key.VideoGameId,
                Title = sale.Key.Title,
                Quantity = sale.Sum(name => name.Quantity),
                TotalSales = sale.Sum(name => name.TotalPrice)
            });

        var sales = new SaleVm();
        foreach (var item in query)
        {
            sales = new SaleVm()
            {
                VideoGameId = item.VideoGameId,
                Title = item.Title,
                Quantity = item.Quantity,
                TotalSales = item.TotalSales
            };
        }

        return View(sales);
    }

    // GET: Sale/DetailsTime/5
    [Route("Admin/Sale/DetailsTime")]
    public async Task<IActionResult> DetailsTime(int? id)
    {
        var orders = await _context.Orders
            .Include(c => c.OrderContents)
            .Include(c => c.Customer)
            .Where(c => c.IsFinal)
            .ToListAsync();

        if (id == 1)
        {
            orders = orders
            .Where(c => c.IsFinal ? c.DeliveryTime.AddDays(1) >= DateTime.Now : c.ArrivalTime.AddDays(1) >= DateTime.Now)
            .ToList();
        }
        else if (id == 2)
        {
            orders = orders
            .Where(c => c.IsFinal ? c.DeliveryTime.AddMonths(1) >= DateTime.Now : c.ArrivalTime.AddMonths(1) >= DateTime.Now)
            .ToList();
        }
        else
        {
            orders = orders
            .Where(c => c.IsFinal ? c.DeliveryTime.AddYears(1) >= DateTime.Now : c.ArrivalTime.AddYears(1) >= DateTime.Now)
            .ToList();
        }

        var ordercontents = _mapper.Map<List<OrderContentVm>>(
            await _context.OrderContents
            .Include(c => c.Order)
            .Include(c => c.Order!.Customer)
            .Include(c => c.VideoGame)
            .Include(c => c.VideoGame!.Offer)
            .ToListAsync());

        ordercontents = ordercontents
            .Where(c => orders.Any(j => j.Id == c.OrderId))
            .ToList();

        var query = ordercontents.GroupBy(name => new { name.VideoGameId, name.VideoGame!.Title })
            .Select(sale => new
            {
                VideoGameId = sale.Key.VideoGameId,
                Title = sale.Key.Title,
                Quantity = sale.Sum(name => name.Quantity),
                TotalSales = sale.Sum(name => name.TotalPrice)
            });

        var sales = new List<SaleVm>();

        foreach (var item in query)
        {
            sales.Add(new SaleVm()
            {
                VideoGameId = item.VideoGameId,
                Title = item.Title,
                Quantity = item.Quantity,
                TotalSales = item.TotalSales
            });
        }

        var output = new SaleTimeVm()
        {
            id = (int)id!,
            ListSales = sales
        };
        return View(output);
    }

    // GET: Sale/DetailsTimeItem/5
    [Route("Admin/Sale/DetailsTimeItem")]
    public async Task<IActionResult> DetailsTimeItem(int? id, int? tid)
    {
        var orders = await _context.Orders
            .Include(c => c.OrderContents)
            .Include(c => c.Customer)
            .Where(c => c.IsFinal)
            .ToListAsync();

        if (tid == 1)
        {
            orders = orders
            .Where(c => c.IsFinal ? c.DeliveryTime.AddDays(1) >= DateTime.Now : c.ArrivalTime.AddDays(1) >= DateTime.Now)
            .ToList();
        }
        else if (tid == 2)
        {
            orders = orders
            .Where(c => c.IsFinal ? c.DeliveryTime.AddMonths(1) >= DateTime.Now : c.ArrivalTime.AddMonths(1) >= DateTime.Now)
            .ToList();
        }
        else
        {
            orders = orders
            .Where(c => c.IsFinal ? c.DeliveryTime.AddYears(1) >= DateTime.Now : c.ArrivalTime.AddYears(1) >= DateTime.Now)
            .ToList();
        }

        var ordercontents = _mapper.Map<List<OrderContentVm>>(
            await _context.OrderContents
            .Include(c => c.Order)
            .Include(c => c.Order!.Customer)
            .Include(c => c.VideoGame)
            .Include(c => c.VideoGame!.Offer)
            .Where(c => c.VideoGameId == id)
            .ToListAsync());

        ordercontents = ordercontents
            .Where(c => orders.Any(j => j.Id == c.OrderId))
            .ToList();

        var query = ordercontents.GroupBy(name => new { name.VideoGameId, name.VideoGame!.Title })
            .Select(sale => new
            {
                VideoGameId = sale.Key.VideoGameId,
                Title = sale.Key.Title,
                Quantity = sale.Sum(name => name.Quantity),
                TotalSales = sale.Sum(name => name.TotalPrice)
            });

        var sales = new SaleTimeItemVm();

        foreach (var item in query)
        {
            sales = new SaleTimeItemVm()
            {
                id = (int)tid!,
                VideoGameId = item.VideoGameId,
                Title = item.Title,
                Quantity = item.Quantity,
                TotalSales = item.TotalSales
            };
        }
        return View(sales);
    }
}
