using Microsoft.AspNetCore.Mvc;
using SportStore.Services.IServices;

namespace SportStore.Components
{
    public class NavigationMenuViewComponent : ViewComponent
    {
        private readonly ICategoryService categoryService;

        public NavigationMenuViewComponent(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }
        public IViewComponentResult Invoke()
        {
            ViewBag.SelectedCategoryId = RouteData?.Values["categoryId"];

            return View(
                categoryService.GetAll()
                    .OrderBy(c => c.Name)
                    .ToList()
            );
        }
    }
}
