using Microsoft.AspNetCore.Mvc;
using OnlineCatalog.ViewModels;

namespace OnlineCatalog.Components
{
    [ViewComponent]
    public class MessageDetails : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(AddMessageViewModel addMessageViewModel)
        {
            return View("../Messages/Details", addMessageViewModel);
        }
    }
}
