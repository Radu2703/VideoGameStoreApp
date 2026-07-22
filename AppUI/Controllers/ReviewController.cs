using AppUI.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AppUI.Models.ViewModels;
using NuGet.Packaging.Signing;
namespace AppUI.Controllers;
public class ReviewController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    public ReviewController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [Route("Reviews")]
    public async Task<IActionResult> Items()
    {
        var VideoGames = await _context.VideoGames
            .Include(t => t.Developers!)
            .ThenInclude(t => t.Developer)
            .Include(t => t.Genre)
            .Include(t => t.Offer)
            .Where(i => i.IsFinal)
            .OrderBy(i => i.Title)
            .ToListAsync();

        var Reviews = await _context.Reviews
            .Include(t => t.Reviewer!)
            .Include(t => t.VideoGame!)
            .Where(i => i.IsFinal && i.VideoGame!.IsFinal)
            .OrderBy(i => i.VideoGameId)
        .ToListAsync();

        var videoGameItems = new List<VideoGame>(VideoGames);

        foreach (var item in VideoGames)
        {
            var r = Reviews.Where(c => c.VideoGameId == item.Id).FirstOrDefault();
            if (r == null)
            {
                videoGameItems.Remove(item);
            }
        }
        return View(_mapper.Map<List<CardVideoGameVm>>(videoGameItems));
    }

    [Route("Review/Index/{id}")]
    public async Task<IActionResult> Index(int? id)
    {
        var Reviews = await _context.Reviews
            .Include(t => t.Reviewer!)
            .Include(t => t.VideoGame!)
            .Where(i => i.IsFinal && i.VideoGame!.IsFinal && i.VideoGameId == id)
            .OrderBy(i => i.VideoGameId)
            .ToListAsync();

        var videoGame = await _context.VideoGames
            .Where(i => i.Id == id)
            .FirstOrDefaultAsync();

        var output = new IndexVideoGameReviewsVm()
        {
            VideoGame = videoGame!.Title,
            ListReviews = _mapper.Map<List<CardReviewVm>>(Reviews)
        };

        return View(output);
    }

    [ActionName("Details")]
    public async Task<IActionResult> Details(int? vid, int? rid)
    {
        var Review = _mapper.Map<ReviewDetailsVm>(await _context.Reviews
            .Include(i => i.VideoGame)
            .Include(i => i.Reviewer)
            .Where(i => i.IsFinal)
            .FirstOrDefaultAsync(i => i.VideoGameId == vid && i.ReviewerId == rid));
        if (Review == null)
            return NotFound();
        /*
        var Reviewers = _mapper.Map<CardReviewerVm>(await _context.Reviewers
            .Where(i => i.Reviews!.Any(i => i.VideoGameId == vid && i.ReviewerId == rid))
            .FirstOrDefaultAsync());
        Review.Reviewer = Reviewers.Name;
        */
        return View(Review);
    }
}