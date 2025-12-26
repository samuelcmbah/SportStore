using Microsoft.AspNetCore.Mvc;
using SportStore.Services.IServices;

namespace SportStore.Components
{
    public class NavigationMenuViewComponent : ViewComponent
    {
        private IStoreRepository repository;

        public NavigationMenuViewComponent(IStoreRepository repo)
        {
            repository = repo;
        }
        public IViewComponentResult Invoke()
        {
            ViewBag.SelectedCategory = RouteData?.Values["category"];
            return View(repository.AllProducts.Select(x => x.Category)
                    .Distinct().OrderBy(x => x));
        }
    }
}
