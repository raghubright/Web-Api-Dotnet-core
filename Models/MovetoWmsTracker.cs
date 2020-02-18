using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovejobtoWms.Models
{


    public class MovetoWmsTracker
    {
        [Key]
        public int Id { get; set; }
        public bool isGetActivityurlCalled { get; set; } = false;

        public string GetActivityResponse { get; set; }

        public bool istrnPropupdated { get; set; } = false;
        public bool isItrackUpdated { get; set; } = false;

        public bool isFileDownloaded { get; set; } = false;
        public bool isrecordUpdated { get; set; } = false;

        public bool isUpdateActivityurlCalled { get; set; } = false;

        public string UpdateActivityResponse { get; set; }
    }
}