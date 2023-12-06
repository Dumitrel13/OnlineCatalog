using OnlineCatalog.Models;

namespace OnlineCatalog.Tests.Resources
{
    public static class YearStructureResources
    {
        public static List<YearStructure> YearStructures => new();

        public static YearStructure YearStructure => new()
        {
            StartingYear = new DateTime(2023, 06, 22), EndingYear = new DateTime(2023, 07, 22),
            Periods = new HashSet<Period>()
        };


    }
}
