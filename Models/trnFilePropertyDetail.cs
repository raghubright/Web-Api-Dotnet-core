using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovejobtoWms.Models
{

    [Table("wms_trnFilePropertyDetails")]
    public class trnFilePropertyDetail
    {
        [Key]
        public int IdUnique { get; set; }
        public short id { get; set; } =195;

        public short bookId { get; set; }

        public int fileId { get; set; }

        public string value { get; set; }

        public string createdby { get; set; } = "IS1820";

        public DateTime createddate { get; set; } = DateTime.Now;

        public string @lock { get; set; } = "N";
    }
}