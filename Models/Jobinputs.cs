namespace MovejobtoWms.Models
{
    public class Jobinputs
    {
        public string JobId { get; set; }
        public int ActivityId { get; set; }
        public string CreatedOn { get; set; }

        public int Status { get; set; }
        public int Wfdid { get; set; }
        public string ArticleName { get; set; }
        public string ArticleCode { get; set; }
        public int JournalId { get; set; }
        public string JournalName { get; set; }

        public int CustomerId { get; set; }
        public string CustomerName { get; set; }

        public string WfdName { get; set; }

        public string path { get; set; }


    }
}