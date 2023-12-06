using OnlineCatalog.Models;

namespace OnlineCatalog.ViewModels
{
    public class DeleteUserViewModel
    {
        public ApplicationUser User { get; set; } = default!;
        public string ActionControllerName { get; set; } = string.Empty;
    }
}
