using Microsoft.IdentityModel.Tokens;
using OnlineCatalog.Models;

namespace OnlineCatalog.Helpers
{
    public class FinalGradeWithoutExam : Strategy
    {
        public override string CalculateFinalGrade(List<Grade> grades, Grade examGrade)
        {
            double sum = 0;
            double count = 0;

            if (grades.IsNullOrEmpty())
            {
                return "0";
            }

            foreach (var grade in grades)
            {
                sum += Convert.ToInt32(grade.Score);
                count++;
            }

            return Math.Round(sum / count, MidpointRounding.AwayFromZero).ToString();
        }

    }
}
