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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AppUI.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = "Administrator")]

public class VideoGamePublisherController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    public VideoGamePublisherController(ApplicationDbContext context, IMapper mapper, UserManager<AppUser> userManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
    }

    [Route("Admin/VideoGamePublishers")]
    public async Task<IActionResult> Index()
    {
        if (_context.Publishers == null)
            return Problem("Entity set 'ApplicationDbContext.Publishers' is null.");

        var videogames = await _context.VideoGames.ToListAsync();

        var emptypubs = await _context.Publishers.ToListAsync();

        var query = videogames.GroupBy(name => new { name.PublisherId })
            .Select(pro => new
            {
                PublisherId = pro.Key.PublisherId,
                Quantity = pro.Count()
            });

        var publishers = new List<VideoGamePublisherVm> ();

        foreach (var item in query)
        {
            var pro = await _context.Publishers
                        .Where(i => i.Id == item.PublisherId)
                        .FirstOrDefaultAsync();

            var name = pro!.Name;

            publishers.Add(new VideoGamePublisherVm()
            {
                PublisherId = (int)item.PublisherId!,
                Name = name!,
                VideoGames = item.Quantity
            });
        }

        foreach (var item in emptypubs)
        {
            var name = item!.Name;
            var sch = publishers.FirstOrDefault(i => i.PublisherId == item.Id);

            if (sch == null)
            {
                publishers.Add(new VideoGamePublisherVm()
                {
                    PublisherId = (int)item.Id!,
                    Name = name!,
                    VideoGames = 0,
                });
            }
        }

        var output = new IndexVideoGameVm()
        {
            ListVideoGamePublishers = publishers.OrderBy(c => c.PublisherId).ToList()
        };
        return View(output);
    }

    // GET: VideoGamePublisher/Details/5
    [Route("Admin/VideoGamePublisher/Details")]
    public async Task<IActionResult> Details(int? id)
    {
        var videogames = _mapper.Map<List<VideoGameVm>>(
            await _context.VideoGames
            .Include(c => c.Genre)
            .Include(c => c.Offer)
            .Where(c => c.PublisherId == id)
            .ToListAsync());

        var pro = await _context.Publishers
                        .Where(i => i.Id == id)
                        .FirstOrDefaultAsync();

        var name = pro!.Name;

        var output = new IndexVideoGamePublisherVm()
        {
            Name = name,
            ListVideoGames = videogames
        };
        return View(output);
    }
}
