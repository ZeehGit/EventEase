using EventEase.Data;
using EventEase.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventEase.Controllers
{
    public class EventsController : Controller
    {
        private readonly AppDbContext _context;

        public EventsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            return View(await _context.Events.ToListAsync());
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var ev = await _context.Events
                .FirstOrDefaultAsync(e => e.EventID == id);

            if (ev == null) return NotFound();

            return View(ev);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventID,Name,StartDate,EndDate,Description,ImageURL")] Event ev)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ev);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(ev);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var ev = await _context.Events.FindAsync(id);

            if (ev == null) return NotFound();

            return View(ev);
        }

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventID,Name,StartDate,EndDate,Description,ImageURL")] Event ev)
        {
            if (id != ev.EventID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ev);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(ev.EventID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(ev);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var ev = await _context.Events
                .FirstOrDefaultAsync(e => e.EventID == id);

            if (ev == null) return NotFound();

            return View(ev);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Block deletion if event has bookings
            bool hasBookings = await _context.Bookings.AnyAsync(b => b.EventID == id);
            if (hasBookings)
            {
                TempData["ErrorMessage"] = "This event cannot be deleted because it has existing bookings.";
                return RedirectToAction(nameof(Index));
            }

            var ev = await _context.Events.FindAsync(id);
            if (ev != null)
            {
                _context.Events.Remove(ev);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventID == id);
        }
    }
}

/*
* Author: Alex Chzhen
* Title: Created, CreatedAtAction, CreatedAtRoute Methods In ASP.NET Core Explained With Examples
* Available at: https://ochzhen.com/blog/created-createdataction-createdatroute-methods-explained-aspnet-core#:~:text=version=1.0%20%7D%20%7D-,CreatedAtAction%20(string%20actionName%2C%20object%20value),./api/Values%20%7D%20%7D
* Accessed date: 11 April 2026
*/