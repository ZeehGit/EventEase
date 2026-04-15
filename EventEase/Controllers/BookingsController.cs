using EventEase.Data;
using EventEase.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EventEase.Controllers
{
    public class BookingsController : Controller
    {
        private readonly AppDbContext _context;

        public BookingsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index()
        {
            var bookings = _context.Bookings
                .Include(b => b.Venue)
                .Include(b => b.Event);
            return View(await bookings.ToListAsync());
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Venue)
                .Include(b => b.Event)
                .FirstOrDefaultAsync(b => b.BookingID == id);

            if (booking == null) return NotFound();

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            ViewData["VenueID"] = new SelectList(_context.Venues, "VenueID", "Name");
            ViewData["EventID"] = new SelectList(_context.Events, "EventID", "Name");
            return View();
        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookingID,VenueID,EventID,BookingDate,Status,CreatedBy")] Booking booking)
        {
            if (ModelState.IsValid)
            {
                // Check for double booking — same venue on same date
                bool conflict = await _context.Bookings.AnyAsync(b =>
                    b.VenueID == booking.VenueID &&
                    b.BookingDate.Date == booking.BookingDate.Date);

                if (conflict)
                {
                    ModelState.AddModelError("", "This venue is already booked on the selected date.");
                    ViewData["VenueID"] = new SelectList(_context.Venues, "VenueID", "Name", booking.VenueID);
                    ViewData["EventID"] = new SelectList(_context.Events, "EventID", "Name", booking.EventID);
                    return View(booking);
                }

                // Generate unique booking reference
                booking.UniqueBookingRef = "BK-" + DateTime.Now.ToString("yyyyMMdd") + "-" + Guid.NewGuid().ToString("N")[..6].ToUpper();

                _context.Add(booking);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["VenueID"] = new SelectList(_context.Venues, "VenueID", "Name", booking.VenueID);
            ViewData["EventID"] = new SelectList(_context.Events, "EventID", "Name", booking.EventID);
            return View(booking);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null) return NotFound();

            ViewData["VenueID"] = new SelectList(_context.Venues, "VenueID", "Name", booking.VenueID);
            ViewData["EventID"] = new SelectList(_context.Events, "EventID", "Name", booking.EventID);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingID,UniqueBookingRef,VenueID,EventID,BookingDate,Status,CreatedBy")] Booking booking)
        {
            if (id != booking.BookingID) return NotFound();

            if (ModelState.IsValid)
            {
                // Check for double booking excluding current record
                bool conflict = await _context.Bookings.AnyAsync(b =>
                    b.VenueID == booking.VenueID &&
                    b.BookingDate.Date == booking.BookingDate.Date &&
                    b.BookingID != booking.BookingID);

                if (conflict)
                {
                    ModelState.AddModelError("", "This venue is already booked on the selected date.");
                    ViewData["VenueID"] = new SelectList(_context.Venues, "VenueID", "Name", booking.VenueID);
                    ViewData["EventID"] = new SelectList(_context.Events, "EventID", "Name", booking.EventID);
                    return View(booking);
                }

                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["VenueID"] = new SelectList(_context.Venues, "VenueID", "Name", booking.VenueID);
            ViewData["EventID"] = new SelectList(_context.Events, "EventID", "Name", booking.EventID);
            return View(booking);
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var booking = await _context.Bookings
                .Include(b => b.Venue)
                .Include(b => b.Event)
                .FirstOrDefaultAsync(b => b.BookingID == id);

            if (booking == null) return NotFound();

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(b => b.BookingID == id);
        }
    }
}

/*
* Author: Alex Chzhen
* Title: Created, CreatedAtAction, CreatedAtRoute Methods In ASP.NET Core Explained With Examples
* Available at: https://ochzhen.com/blog/created-createdataction-createdatroute-methods-explained-aspnet-core#:~:text=version=1.0%20%7D%20%7D-,CreatedAtAction%20(string%20actionName%2C%20object%20value),./api/Values%20%7D%20%7D
* Accessed date: 11 April 2026
*/