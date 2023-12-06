using OnlineCatalog.Models;

namespace OnlineCatalog.Helpers
{
    public class FinalGradeCalculator
    {
        private Strategy _strategy;

        public void SetStrategy(Strategy strategy)
        {
            _strategy = strategy;
        }

        public string GetFinalGrade(List<Grade> grades, Grade? examGrade)
        {
            var finalGrade = _strategy.CalculateFinalGrade(grades, examGrade);

            return finalGrade;
        }
    }
}
