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
            ViewBag.SelectedCategory = RouteData?.Values["category"];

            return View(
                categoryService.GetAll()
                    .OrderBy(c => c.Name)
                    .Select(c => c.Name)
            );
        }
    }
}
