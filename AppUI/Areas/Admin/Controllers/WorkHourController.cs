using AppUI.Models.CustomIdentity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using AppUI.Areas.Admin.Models.DTOs;
using AppUI.Areas.Admin.Models.ViewModels;
using AppUI.Areas.Manager.Models.DTOs;
using AppUI.Models.Entities;
namespace AppUI.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Administrator")]
public class WorkHourController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;

    public WorkHourController(ApplicationDbContext context, IMapper mapper, UserManager<AppUser> userManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
    }

    // GET: Admin/WorkHour
    public async Task<IActionResult> Index()
    {
        if (_context.WorkHours == null)
            return Problem("Entity set 'ApplicationDbContext.WorkHours' is null.");
        return View(_mapper.Map<List<ExistentWorkHourDto>>(await _context.WorkHours.ToListAsync()));
    }

    // GET: Admin/WorkHour/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null || _context.WorkHours == null)
        {
            return NotFound();
        }

        var ExistentWorkHour = _mapper.Map<ExistentWorkHourDto>(await _context.WorkHours.FirstOrDefaultAsync(m => m.Id == id));
        if (ExistentWorkHour == null)
        {
            return NotFound();
        }

        return View(ExistentWorkHour);
    }

    // GET: Admin/WorkHour/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || _context.WorkHours == null)
        {
            return NotFound();
        }

        var ExistentWorkHour = _mapper.Map<ExistentWorkHourDto>(await _context.WorkHours.FindAsync(id));
        if (ExistentWorkHour == null)
        {
            return NotFound();
        }
        return View(ExistentWorkHour);
    }

    // POST: Admin/WorkHour/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [FromForm] ExistentWorkHourDto modifiedWorkHour)
    {
        if (id != modifiedWorkHour.Id)
        {
            return NotFound();
        }
        var ExistentWorkHour = await _context.WorkHours.FindAsync(id);
        if (ExistentWorkHour == null)
        {
            return NotFound();
        }
        if (ExistentWorkHour.Id != modifiedWorkHour.Id)
        {
            return BadRequest();
        }

        if (ModelState.IsValid)
        {
            var employeeschedule = _mapper.Map<List<EmployeeScheduleVm>>(
                await _context.EmployeeSchedules
                .Include(c => c.Employee)
                .Include(c => c.Day)
                .Where(c => c.DayId == id && (modifiedWorkHour.IsWorkDay == false))
                .OrderBy(c => c.DayId)
                .ThenBy(c => c.StartHour)
                .ThenBy(c => c.EndHour)
                .ThenBy(c => c.Employee!.Email)
                .ToListAsync());

            if (employeeschedule != null && employeeschedule.Count > 0)
            {
                TempData["MessageWorkHour"] = $"An employee still works on <strong>{ExistentWorkHour.Name}</strong>. Remove it before you remove the work day.";
                return RedirectToAction(nameof(Index));
            }

            var employeehours = _mapper.Map<List<EmployeeScheduleVm>>(
                await _context.EmployeeSchedules
                .Include(c => c.Employee)
                .Include(c => c.Day)
                .Where(c => c.DayId == id && (c.StartHour < modifiedWorkHour.StartHour || c.EndHour < modifiedWorkHour.StartHour || c.StartHour > modifiedWorkHour.EndHour || c.EndHour > modifiedWorkHour.EndHour))
                .OrderBy(c => c.DayId)
                .ThenBy(c => c.StartHour)
                .ThenBy(c => c.EndHour)
                .ThenBy(c => c.Employee!.Email)
                .ToListAsync());

            if (employeehours != null && employeehours.Count > 0)
            {
                TempData["MessageWorkHour"] = $"An employee's schedule wouldn't fit in the new work hours for <strong>{ExistentWorkHour.Name}</strong>!";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                TempData["MessageWorkHour"] = $"We modified work hours for <strong>{ExistentWorkHour.Name}</strong>!";
                modifiedWorkHour.ToEntity(ref ExistentWorkHour);
                _context.Update(ExistentWorkHour);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkHourExists(ExistentWorkHour.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(modifiedWorkHour);
    }

    private bool WorkHourExists(int id)
    {
        return (_context.WorkHours?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}