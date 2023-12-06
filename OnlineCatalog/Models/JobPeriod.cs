using System.ComponentModel.DataAnnotations;

namespace OnlineCatalog.Models
{
    public class JobPeriod
    {
        [Key]
        public virtual int JobPeriodId { get; set; }
        public virtual DateTime StartingDate { get; set; }
        public virtual DateTime? EndingDate { get; set; }
    }
}
