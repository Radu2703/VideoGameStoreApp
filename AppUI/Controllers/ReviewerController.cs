using AppUI.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppUI.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using AppUI.Areas.Admin.Models.DTOs;
using AppUI.Models.CustomIdentity;
namespace AppUI.Controllers;
public class ReviewerController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    public readonly SignInManager<AppUser> _signInManager;
    public readonly UserManager<AppUser> _userManager;
    public ReviewerController(ApplicationDbContext context, IMapper mapper, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [Route("Reviewers")]
    public async Task<IActionResult> Index()
    {
        var reviewers = await _context.Reviewers.ToListAsync();
        return View(_mapper.Map<List<CardReviewerVm>>(reviewers));
    }

    [Route("Reviewer/Details/{id}")]
    public async Task<IActionResult> Details(int id)
    {
        var Reviewer = await _context.Reviewers.FirstOrDefaultAsync(i => i.Id == id);
        if (Reviewer == null)
            return NotFound();
        var Reviews = await _context.Reviews
            .Include(t => t.Reviewer)
            .Include(t => t.VideoGame)
            .Where(i => i.ReviewerId! == id && i.IsFinal)
            .OrderBy(i => i.VideoGameId)
            .ToListAsync();
        var output = new ReviewerReviewsVm()
        {
            ReviewerDetails = _mapper.Map<CardReviewerVm>(Reviewer),
            Reviews = _mapper.Map<List<CardReviewVm>>(Reviews)
        };
        return View(output);
    }
}