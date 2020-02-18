using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovejobtoWms.Models
{

    [Table("WMS_mstActivityDetails")]
    public class ActivityDetail
    {
        [Key]
        public short activityid { get; set; }

        public string folderName { get; set; }

        public string activitycode { get; set; }
    }
}