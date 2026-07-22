using AppUI.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppUI.Models.ViewModels;
namespace AppUI.Controllers;
public class NewSuggestionController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    public NewSuggestionController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [Route("NewSuggestions")]
    public async Task<IActionResult> Index()
    {
        var CurrentCustomer = await _context.Users.FirstOrDefaultAsync(q => q.UserName == User.Identity!.Name);
        if (CurrentCustomer == null || CurrentCustomer.UserName == null)
            return Redirect("Identity/Account/Login");

        var OwnedOrders = await _context.Orders
            .Include(c => c.Customer)
            .Include(c => c.PayingMethod)
            .Include(c => c.OrderContents)
            .Where(q => q.CustomerId == CurrentCustomer.Id && q.IsFinal)
            .ToListAsync();
        
        var OwnedOrderContents = await _context.OrderContents
            .Include(c => c.VideoGame)
            .ToListAsync();

        OwnedOrderContents = OwnedOrderContents
            .Where(q => OwnedOrders.Any(j => j.Id == q.OrderId))
            .ToList();

        var OwnedVideoGames = await _context.VideoGames
            .Include(t => t.Publisher)
            .Include(t => t.Developers!)
            .ThenInclude(t => t.Developer)
            .Include(t => t.Genre)
            .Include(t => t.Offer)
            .OrderBy(i => i.Title)
            .ToListAsync();

        OwnedVideoGames = OwnedVideoGames
            .Where(i => OwnedOrderContents.Any(j => j.VideoGameId == i.Id))
            .OrderBy(i => i.Title)
            .ToList();

        var Suggestions = await _context.VideoGames
            .Include(t => t.Publisher)
            .Include(t => t.Developers!)
            .ThenInclude(t => t.Developer)
            .Include(t => t.Genre)
            .Include(t => t.Offer)
            .OrderBy(i => i.Title)
            .ToListAsync();

        Suggestions = Suggestions
            .Where(i => !OwnedVideoGames.Any(j => j.Id == i.Id) && i.IsFinal)
            .OrderBy(i => i.Title)
            .ToList();
        return View(_mapper.Map<List<CardVideoGameVm>>(Suggestions));
    }
    /*
    [Route("VideoGame/Details/{id}")]
    public async Task<IActionResult> Details(int id)
    {
        var VideoGame = _mapper.Map<VideoGameDetailsVm>(await _context.VideoGames
            .Include(t => t.Offer)
            .Include(t => t.Category)
            .Where(i => i.IsFinal)
            .FirstOrDefaultAsync(i => i.Id == id));
        if (VideoGame == null)
            return NotFound();
        var Developers = _mapper.Map<List<CardDeveloperVm>>(await _context.Developers
            .Where(i => i.VideoGames!.Any(i => i.VideoGameId == id))
            .ToListAsync());
        VideoGame.ListDevelopers = Developers;
        return View(VideoGame);
    }

    [Route("Offers")]
    public async Task<IActionResult> Offers()
    {
        var VideoGames = await _context.VideoGames
            .Include(t => t.Developers!)
            .ThenInclude(t => t.Developer)
            .Include(t => t.Offer)
            .Where(i => i.IsFinal && i.Offer != null)
            .OrderBy(i => i.Title)
            .ToListAsync();
        return View(_mapper.Map<List<CardVideoGameVm>>(VideoGames));
    }
    */
}