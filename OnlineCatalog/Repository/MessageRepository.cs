using Microsoft.EntityFrameworkCore;
using OnlineCatalog.Models;
using OnlineCatalog.Data;
using OnlineCatalog.Repository.Interfaces;

namespace OnlineCatalog.Repository
{
    public class MessageRepository : BaseRepository<Message>, IMessageRepository
    {
        public MessageRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<Message>> GetMessagesForSpecificYearByPupilId(int pupilId, DateTime start, DateTime end)
        {
            return await _dbSet.Where(m => m.Pupil.Id == pupilId && m.Date >= start && m.Date <= end).ToListAsync();
        }

        public async Task<List<Message>> GetMessagesForSpecificYearByTeacherId(int teacherId, DateTime start, DateTime end)
        {
            return await _dbSet.Where(m => m.Teacher.Id == teacherId && m.Date >= start && m.Date <= end).ToListAsync();
        }

        public async Task<Message> GetMessageById(int messageId)
        {
            return await _dbSet.Include("Teacher").Include("Pupil").SingleAsync(m => m.MessageId == messageId);
        }
    }
}
