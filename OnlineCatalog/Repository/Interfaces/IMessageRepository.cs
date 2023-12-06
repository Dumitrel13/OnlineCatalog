using OnlineCatalog.Models;

namespace OnlineCatalog.Repository.Interfaces
{
    public interface IMessageRepository : IBaseRepository<Message>
    {
        Task<List<Message>> GetMessagesForSpecificYearByPupilId(int pupilId, DateTime start, DateTime end);
        Task<List<Message>> GetMessagesForSpecificYearByTeacherId(int teacherId, DateTime start, DateTime end);
        Task<Message> GetMessageById(int messageId);
    }
}
