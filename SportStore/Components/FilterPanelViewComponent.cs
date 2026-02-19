using Microsoft.AspNetCore.Mvc;

namespace SportStore.Components
{
    public class FilterPanelViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
