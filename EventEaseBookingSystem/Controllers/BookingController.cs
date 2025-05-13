using EventEaseBookingSystem.Models;

using Humanizer;

using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
 

namespace EventEaseBookingSystem.Controllers

{

    public class BookingController : Controller

    {

        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)

        {

            _context = context;

        }

        public async Task<IActionResult> Index(string searchString)

        {

            var bookings = _context.Booking

                .Include(b => b.Event)

                .Include(b => b.Venue)

                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))

            {

                bookings = bookings.Where(b =>

                    b.Venue.VenueName.Contains(searchString) ||

                    b.Event.EventName.Contains(searchString));

            }


            return View(await bookings.ToListAsync());

        }

        public IActionResult Create()

        {

            ViewData["Events"] = _context.Event.ToList();

            ViewData["Venues"] = _context.Venue.ToList();

            return View();

        }

        [HttpPost]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(Booking booking)

        {

            var selectedEvent = await _context.Event.FirstOrDefaultAsync(e => e.EventID == booking.EventID);

            if (selectedEvent == null)

            {

                ModelState.AddModelError("", "Selected event not found.");

                ViewData["Events"] = _context.Event.ToList();

                ViewData["Venues"] = _context.Venue.ToList();

                return View(booking);

            }

            // Check manually for double booking

            var conflict = await _context.Booking

                .Include(b => b.Event)

                .AnyAsync(b => b.VenueID == booking.VenueID &&

                               b.Event.EventDate.Date == selectedEvent.EventDate.Date);

            if (conflict)

            {

                ModelState.AddModelError("", "This venue is already booked for that date.");

                ViewData["Events"] = _context.Event.ToList();

                ViewData["Venues"] = _context.Venue.ToList();

                return View(booking);

            }

            if (ModelState.IsValid)

            {

                try

                {

                    _context.Add(booking);

                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Booking created successfully.";

                    return RedirectToAction(nameof(Index));

                }

                catch (DbUpdateException ex)

                {

                    // If database constraint fails (e.g., unique key violation), show friendly message

                    ModelState.AddModelError("", "This venue is already booked for that date.");

                    ViewData["Events"] = _context.Event.ToList();

                    ViewData["Venues"] = _context.Venue.ToList();

                    return View(booking);

                }

            }

            ViewData["Events"] = _context.Event.ToList();

            ViewData["Venues"] = _context.Venue.ToList();

            return View(booking);

        }

        public async Task<IActionResult> Details(int? id)

        {

            if (id == null)

            {

                return NotFound();

            }

            var booking = await _context.Booking

                .Include(b => b.Event)

                .Include(b => b.Venue)

                .FirstOrDefaultAsync(m => m.BookingID == id);

            if (booking == null)

            {

                return NotFound();

            }

            return View(booking);

        }

        // Edit Action Methods: This method retrieves the booking details for the specified id and returns them to the view for editing.

        public async Task<IActionResult> Edit(int? id)

        {

            if (id == null)

            {

                return NotFound();

            }

            var booking = await _context.Booking.FindAsync(id);

            if (booking == null)

            {

                return NotFound();

            }

            ViewData["Events"] = _context.Event.ToList();

            ViewData["Venues"] = _context.Venue.ToList();

            return View(booking);

        }

        // POST: Edit (This method processes the edited booking details and updates the record in the database.)

        [HttpPost]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(int id, Booking booking)

        {

            if (id != booking.BookingID)

            {

                return NotFound();

            }

            if (ModelState.IsValid)

            {

                try

                {

                    _context.Update(booking);

                    await _context.SaveChangesAsync();

                }

                catch (DbUpdateConcurrencyException)

                {

                    if (!_context.Booking.Any(e => e.BookingID == id))

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

            ViewData["Events"] = _context.Event.ToList();

            ViewData["Venues"] = _context.Venue.ToList();

            return View(booking);

        }

        // Delete Action Methods

        //GET: Delete (This method retrieves the booking details for the specified id and returns them to the view for deletion confirmation.)

        public async Task<IActionResult> Delete(int? id)

        {

            if (id == null)

            {

                return NotFound();

            }

            var booking = await _context.Booking

                .Include(b => b.Event)

                .Include(b => b.Venue)

                .FirstOrDefaultAsync(m => m.BookingID == id);

            if (booking == null)

            {

                return NotFound();

            }

            return View(booking);

        }

        // POST: Delete (This method deletes the booking record from the database.)

        [HttpPost, ActionName("Delete")]

        [ValidateAntiForgeryToken]

        public async Task<IActionResult> DeleteConfirmed(int id)

        {

            var booking = await _context.Booking.FindAsync(id);

            if (booking != null)

            {

                _context.Booking.Remove(booking);

                await _context.SaveChangesAsync();

            }

            return RedirectToAction(nameof(Index));

        }

    }

}
