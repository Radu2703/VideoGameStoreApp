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

public class VideoGameReviewerController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    public VideoGameReviewerController(ApplicationDbContext context, IMapper mapper, UserManager<AppUser> userManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
    }

    [Route("Admin/VideoGameReviewers")]
    public async Task<IActionResult> Index()
    {
        if (_context.Reviewers == null)
            return Problem("Entity set 'ApplicationDbContext.Reviewers' is null.");

        var reviews = await _context.Reviews.ToListAsync();

        var emptyrevs = await _context.Reviewers.ToListAsync();

        var query = reviews.GroupBy(name => new { name.ReviewerId })
            .Select(pro => new
            {
                ReviewerId = pro.Key.ReviewerId,
                Quantity = pro.Count()
            });

        var reviewers = new List<ReviewerVm> ();

        foreach (var item in query)
        {
            var pro = await _context.Reviewers
                        .Where(i => i.Id == item.ReviewerId)
                        .FirstOrDefaultAsync();

            var name = pro!.Name;

            reviewers.Add(new ReviewerVm()
            {
                ReviewerId = (int)item.ReviewerId!,
                Name = name!,
                Reviews = item.Quantity
            });
        }

        foreach (var item in emptyrevs)
        {
            var name = item!.Name;
            var sch = reviewers.FirstOrDefault(i => i.ReviewerId == item.Id);

            if (sch == null)
            {
                reviewers.Add(new ReviewerVm()
                {
                    ReviewerId = (int)item.Id!,
                    Name = name!,
                    Reviews = 0,
                });
            }
        }

        var output = new IndexReviewerVm()
        {
            ListReviewers = reviewers.OrderBy(c => c.ReviewerId).ToList()
        };
        return View(output);
    }

    // GET: VideoGameReviewer/Details/5
    [Route("Admin/VideoGameReviewer/Details")]
    public async Task<IActionResult> Details(int? id)
    {
        var reviews = _mapper.Map<List<ReviewVm>>(
            await _context.Reviews
            .Include(c => c.VideoGame)
            .Where(c => c.ReviewerId == id)
            .ToListAsync());

        var rev = await _context.Reviewers
                        .Where(i => i.Id == id)
                        .FirstOrDefaultAsync();

        var name = rev!.Name;

        var output = new IndexVideoGameReviewerVm()
        {
            Name = name,
            ListReviews = reviews
        };
        return View(output);
    }
}
