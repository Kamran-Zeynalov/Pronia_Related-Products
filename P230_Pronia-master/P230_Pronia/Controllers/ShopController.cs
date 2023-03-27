using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P230_Pronia.DAL;
using P230_Pronia.Entities;
using System.Numerics;

namespace P230_Pronia.Controllers
{
    public class ShopController : Controller
    {
        private readonly ProniaDbContext _context;

        public ShopController(ProniaDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<Plant> plants = _context.Plants.Include(P => P.PlantImages)
                .Include(P => P.PlantDeliveryInformation)
                .Include(P => P.PlantCategories)
                .ThenInclude(P => P.Category)
                .Include(P => P.PlantTags)
                .ThenInclude(P => P.Tag)
                .ToList();

            ViewBag.RelatedProducts = _context.Plants
                .Include(p => p.PlantImages)
                .Take(12)
                .ToList();

            return View(plants);
        }

        public IActionResult Index1(int id)
        {
            if (id == 0) return NotFound();
            Plant? plant = _context.Plants.Include(P => P.PlantImages)
                .Include(P => P.PlantDeliveryInformation)
                .Include(P => P.PlantCategories)
                .ThenInclude(P => P.Category)
                .Include(P => P.PlantTags)
                .ThenInclude(P => P.Tag).FirstOrDefault(P => P.Id == id);

            List<int> categoryIds = plant.PlantCategories.Select(pc => pc.Category.Id).ToList();
            List<Plant> relatedproducts = _context.Plants
                 .Include(rp => rp.PlantImages)
                 .Include(rp => rp.PlantCategories)
                 .ThenInclude(rp => rp.Category)
                 .Where(rp => rp.PlantCategories.Any(pc => categoryIds.Contains(pc.Category.Id)))
                 .Where(rp => rp.Id != plant.Id)
                 .Take(4)
                 .ToList();
            ViewBag.RelatedProducts = relatedproducts;

            if (plant is null) return NotFound();
            return View(plant);
        }
    }
}
