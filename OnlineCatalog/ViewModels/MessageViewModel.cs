using OnlineCatalog.Models;

namespace OnlineCatalog.ViewModels
{
    public class MessageViewModel
    {
        public string PupilFullName { get; set; } = string.Empty;
        public List<Message> Messages { get; set; } = default!;
    }
}
