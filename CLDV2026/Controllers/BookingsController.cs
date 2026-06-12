using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CLDV2026.Data;
using CLDV2026.Models;

namespace CLDV2026.Controllers
{
    public class BookingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        // ✅ Only ONE constructor
        public BookingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Bookings
        public async Task<IActionResult> Index(
    string? searchTerm,
    int? eventTypeId,
    bool? isAvailable,
    DateTime? startDate,
    DateTime? endDate)
        {
            var query = _context.Bookings
                .Include(b => b.Event)
                    .ThenInclude(e => e.EventType)
                .Include(b => b.Venue)
                .AsQueryable();

            // Search by Booking ID or Event Name
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(b =>
                    b.BookingId.ToString().Contains(searchTerm) ||
                    b.Event!.EventName.Contains(searchTerm));
            }

            // Event Type Filter
            if (eventTypeId.HasValue)
            {
                query = query.Where(b =>
                    b.Event!.EventTypeId == eventTypeId);
            }

            // Venue Availability Filter
            if (isAvailable.HasValue)
            {
                query = query.Where(b =>
                    b.Venue!.IsAvailable == isAvailable.Value);
            }

            // Date Range Filter
            if (startDate.HasValue)
            {
                query = query.Where(b =>
                    b.BookingDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(b =>
                    b.BookingDate <= endDate.Value);
            }

            // Preserve filter values
            ViewBag.SearchTerm = searchTerm;
            ViewBag.EventTypeId = eventTypeId;
            ViewBag.IsAvailable = isAvailable;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");

            ViewBag.EventTypes = await _context.EventTypes.ToListAsync();

            return View(await query.ToListAsync());
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            ViewBag.EventId = new SelectList(_context.Events, "EventId", "EventName");
            ViewBag.VenueId = new SelectList(_context.Venues, "VenueId", "VenueName");
            return View();
        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            if (ModelState.IsValid)
            {
                _context.Bookings.Add(booking); // ✅ more explicit
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.EventId = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
            ViewBag.VenueId = new SelectList(_context.Venues, "VenueId", "VenueName", booking.VenueId);
            return View(booking);
        }
    }
}