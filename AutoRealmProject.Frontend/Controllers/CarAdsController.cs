using AutoRealmProject.Backend.Data;
using AutoRealmProject.Backend.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AutoRealmProject.Frontend.Controllers
{
    public class CarAdsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        private static readonly Dictionary<string, List<string>> CarData = new Dictionary<string, List<string>>
        {
            { "Rolls-Royce", new List<string> { "Ghost", "Phantom Series II", "Spectre" } },
            { "Porsche", new List<string> { "911", "Cayenne", "Panamera" } },
            { "Ford", new List<string> { "Fiesta", "Focus", "Mustang" } },
        };

        public CarAdsController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.CarAds.ToListAsync());
        }

        public async Task<IActionResult> Home()
        {
            return View(await _context.CarAds.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carAd = await _context.CarAds
                .Include(c => c.Owner)
                .FirstOrDefaultAsync(m => m.AdId == id);

            if (carAd == null)
            {
                return NotFound();
            }

            return View(carAd);
        }

        [Authorize]
        public IActionResult Create()
        {
            ViewBag.Brands = new SelectList(CarData.Keys);
            return View(new CarAd());
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AdId,Brand,Model,Year,Price,Mileage,City,Description,CarPhoto")] CarAd carAd, IFormFile photo)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                carAd.OwnerId = user.Id;

                if (string.IsNullOrEmpty(carAd.Description))
                {
                    carAd.Description = "No description";
                }

                if (photo == null || photo.Length == 0)
                {
                    ModelState.AddModelError("Photo", "Photo is required.");
                    ViewBag.Brands = new SelectList(CarData.Keys);
                    return View(carAd);
                }

                using (var memoryStream = new MemoryStream())
                {
                    await photo.CopyToAsync(memoryStream);
                    carAd.CarPhoto = memoryStream.ToArray();
                }

                _context.Add(carAd);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Brands = new SelectList(CarData.Keys);
            return View(carAd);
        }

        [HttpGet]
        public JsonResult GetModels(string brand)
        {
            if (CarData.ContainsKey(brand))
            {
                return Json(CarData[brand]);
            }
            return Json(new List<string>());
        }

        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carAd = await _context.CarAds.FindAsync(id);
            ViewBag.Brands = new SelectList(CarData.Keys);
            if (carAd == null)
            {
                return NotFound();
            }
            return View(carAd);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AdId,Brand,Model,Year,Price,Mileage,City,Description")] CarAd carAd, IFormFile newPhoto)
        {
            if (id != carAd.AdId)
            {
                return NotFound();
            }

            try
            {
                if (newPhoto != null && newPhoto.Length > 0)
                {
                    using var memoryStream = new MemoryStream();
                    await newPhoto.CopyToAsync(memoryStream);
                    carAd.CarPhoto = memoryStream.ToArray();
                }
                else
                {
                    var existingCarAd = await _context.CarAds.AsNoTracking().FirstOrDefaultAsync(c => c.AdId == id);
                    if (existingCarAd != null)
                    {
                        carAd.CarPhoto = existingCarAd.CarPhoto;
                    }
                }

                carAd.OwnerId = _userManager.GetUserId(User);

                _context.Update(carAd);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                return View(carAd);
            }
        }

        [Authorize]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var carAd = await _context.CarAds
                .FirstOrDefaultAsync(m => m.AdId == id);
            if (carAd == null)
            {
                return NotFound();
            }

            return View(carAd);
        }

        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var carAd = await _context.CarAds.FindAsync(id);
            if (carAd != null)
            {
                _context.CarAds.Remove(carAd);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarAdExists(int id)
        {
            return _context.CarAds.Any(e => e.AdId == id);
        }
    }
}
