using OnlineCatalog.Models;

namespace OnlineCatalog.Helpers
{
    public abstract class Strategy
    {
        public abstract string CalculateFinalGrade(List<Grade> grades, Grade examGrade);
    }
}
