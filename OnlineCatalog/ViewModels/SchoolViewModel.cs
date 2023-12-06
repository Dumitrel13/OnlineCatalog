using OnlineCatalog.Models;

namespace OnlineCatalog.ViewModels
{
    public class SchoolViewModel
    {
        public School School { get; set; } = default!;

        public string Message { get; set; } = string.Empty;

    }
}
