using AppUI.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppUI.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using AppUI.Areas.Admin.Models.DTOs;
using AppUI.Models.CustomIdentity;
namespace AppUI.Controllers;
public class PublisherController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    public readonly SignInManager<AppUser> _signInManager;
    public readonly UserManager<AppUser> _userManager;
    public PublisherController(ApplicationDbContext context, IMapper mapper, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [Route("Publishers")]
    public async Task<IActionResult> Index()
    {
        var publishers = await _context.Publishers.ToListAsync();
        return View(_mapper.Map<List<CardPublisherVm>>(publishers));
    }

    [Route("Publisher/Details/{id}")]
    public async Task<IActionResult> Details(int id)
    {
        var Publisher = await _context.Publishers.FirstOrDefaultAsync(i => i.Id == id);
        if (Publisher == null)
            return NotFound();
        var VideoGames = await _context.VideoGames
            .Include(t => t.Publisher)
            .Include(t => t.Offer)
            .Where(i => i.PublisherId! == id && i.IsFinal)
            .OrderBy(i => i.Title)
            .ToListAsync();
        var output = new PublisherVideoGamesVm()
        {
            PublisherDetails = _mapper.Map<CardPublisherVm>(Publisher),
            VideoGames = _mapper.Map<List<CardVideoGameVm>>(VideoGames)
        };
        return View(output);
    }
}