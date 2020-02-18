using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovejobtoWms.Models
{


    public class SP_BMJ_JobDetails
    {
        public string JournalName { get; set; }
        public string JournalCode { get; set; }
        public string ArticleName { get; set; }
        public string ArticleCode { get; set; }
        public string ArticleTitle { get; set; }
        public string PEUserName { get; set; }
        public string PEUserEmail { get; set; }
        public string PMUserName { get; set; }
        public string PMUserEmail { get; set; }

    }
    public class BMJ_MstCustomerStageDetails
    {
        public int WMSActivityId { get; set; }

        public int WMSStageId { get; set; }

        public string CustomerStage { get; set; }

        public int SeqId { get; set; }
        public char isLock { get; set; }

    }

    public class SP_BMJ_AssignedJobDetails
    {
        public string bookcode { get; set; }
        public string DOI_Number { get; set; }
        public string ArticleType { get; set; }
        public int ArticleId { get; set; }
        public string Author { get; set; }
        public string Licence { get; set; }
        public string ArticleTitle { get; set; }
    }



}