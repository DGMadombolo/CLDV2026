using CLDV2026.Data;
using CLDV2026.Models;
using CLDV2026.Services;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CLDV2026.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly BlobStorageService _blobStorageService;

        public EventsController(ApplicationDbContext context, BlobStorageService blobStorageService)
        {
            _context = context;
            _blobStorageService = blobStorageService;
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Events.Include(e => e.Venue);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evt = await _context.Events
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(m => m.EventId == id);

            if (evt == null)
            {
                return NotFound();
            }

            return View(evt);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName");
            ViewData["EventTypeId"] = new SelectList(_context.EventTypes, "EventTypeId", "Name");
            return View();
        }

        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventId,EventName,EventDate,Description,VenueId,ImageUrl,EventTypeId")] Event evt, IFormFile ImageFile)
        {
            if (!ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var imageUrl = await _blobStorageService.UploadFileAsync(ImageFile);
                    evt.ImageUrl = imageUrl;
                }
                _context.Add(evt);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", evt.VenueId);
            return View(evt);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evt = await _context.Events.FindAsync(id);
            if (evt == null)
            {
                return NotFound();
            }

            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", evt.VenueId);
            ViewData["EventTypeId"] = new SelectList(_context.EventTypes, "EventTypeId", "Name");
            return View(evt);
        }

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventId,EventName,EventDate,Description,VenueId,ImageUrl,EventTypeId")] Event evt, IFormFile ImageFile)
        {
            if (id != evt.EventId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                try
                {
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        // Delete old image if exists
                        if (!string.IsNullOrEmpty(evt.ImageUrl))
                        {
                            var oldBlobName = new Uri(evt.ImageUrl).Segments.Last();
                            await _blobStorageService.DeleteFileAsync(oldBlobName);
                        }

                        // Upload new image
                        var imageUrl = await _blobStorageService.UploadFileAsync(ImageFile);
                        evt.ImageUrl = imageUrl;
                    }
                    _context.Update(evt);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(evt.EventId))
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

            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", evt.VenueId);
            ViewData["EventTypeId"] = new SelectList(_context.EventTypes, "EventTypeId", "Name");
            return View(evt);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var evt = await _context.Events
                .Include(e => e.Venue)
                .FirstOrDefaultAsync(m => m.EventId == id);

            if (evt == null)
            {
                return NotFound();
            }

            return View(evt);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var evt = await _context.Events.FindAsync(id);
            if (evt != null)
            {
                _context.Events.Remove(evt);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventId == id);
        }
    }
}