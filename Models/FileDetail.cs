using  System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovejobtoWms.Models
{
       
    [Table("wms_mstfiledetails")]
    public class FileDetail
    {
        [Key]
      public int fileID { get; set; }

      public int bookid { get; set; }
      
      public string  fileName { get; set; }
      
      public string @lock { get; set; }
    }
}