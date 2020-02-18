using     System.ComponentModel.DataAnnotations;
using     System.ComponentModel.DataAnnotations.Schema;

namespace MovejobtoWms.Models
{

          [Table("WMS_mstStageDetails")]
          public class StageDetail
          {
          [Key]
          public short stageid { get; set; }
          public string sfoldername { get; set; }
          public string stagecode { get; set; }
          }
}