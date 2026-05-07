using EventEase.Data;
using EventEase.Models;
using EventEase.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventEase.Controllers
{
    public class EventsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly BlobStorageService _blobService;

        public EventsController(AppDbContext context, BlobStorageService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Events.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var ev = await _context.Events.FirstOrDefaultAsync(e => e.EventID == id);
            if (ev == null) return NotFound();
            return View(ev);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventID,Name,StartDate,EndDate,Description")] Event ev, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    ev.ImageURL = await _blobService.UploadImageAsync(imageFile);
                }

                _context.Add(ev);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(ev);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var ev = await _context.Events.FindAsync(id);
            if (ev == null) return NotFound();
            return View(ev);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventID,Name,StartDate,EndDate,Description,ImageURL")] Event ev, IFormFile? imageFile)
        {
            if (id != ev.EventID) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(ev.ImageURL))
                            await _blobService.DeleteImageAsync(ev.ImageURL);

                        ev.ImageURL = await _blobService.UploadImageAsync(imageFile);
                    }

                    _context.Update(ev);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Event updated successfully.";
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

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var ev = await _context.Events.FirstOrDefaultAsync(e => e.EventID == id);
            if (ev == null) return NotFound();
            return View(ev);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            bool hasBookings = await _context.Bookings.AnyAsync(b => b.EventID == id);
            if (hasBookings)
            {
                TempData["ErrorMessage"] = "This event cannot be deleted because it has existing bookings.";
                return RedirectToAction(nameof(Index));
            }

            var ev = await _context.Events.FindAsync(id);
            if (ev != null)
            {
                if (!string.IsNullOrEmpty(ev.ImageURL))
                    await _blobService.DeleteImageAsync(ev.ImageURL);

                _context.Events.Remove(ev);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventID == id);
        }
    }
}