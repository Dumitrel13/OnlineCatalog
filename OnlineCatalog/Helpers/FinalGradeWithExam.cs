using Microsoft.IdentityModel.Tokens;
using OnlineCatalog.Models;

namespace OnlineCatalog.Helpers
{
    public class FinalGradeWithExam : Strategy
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

            var average = sum / count;

            return Math.Round((3 * average + Convert.ToInt32(examGrade.Score)) / 4, MidpointRounding.AwayFromZero).ToString();
        }
    }
}
