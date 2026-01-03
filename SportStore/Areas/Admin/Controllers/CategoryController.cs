using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportStore.Data;
using SportStore.Models;
using SportStore.Services.IServices;

namespace SportStore.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class CategoryController : Controller
    {
        private readonly IStoreRepository storeRepository;
        private readonly StoreDbContext context;

        public CategoryController(IStoreRepository storeRepository, StoreDbContext context)
        {
            this.storeRepository = storeRepository;
            this.context = context;
        }

        public IActionResult Index()
        {
            var categories = storeRepository.Categories
                .OrderBy(c => c.Name) .ToList();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            context.Categories.Add(category);
            context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(int id)
        {
            var category = context.Categories.Find(id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category category)
        {
            if (!ModelState.IsValid)
            { 
                return View(category);
            }
            context.Categories.Update(category);
            context.SaveChanges();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var category = context.Categories.Find(id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var category = context.Categories.Find(id);

            if (category != null)
            {
                context.Categories.Remove(category);
                context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
