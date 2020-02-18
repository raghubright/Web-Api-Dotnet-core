using System;
using  System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovejobtoWms.Models
{
    [Table("wms_mstrecorddetails_36")]
    public class Recorddetail
    {
      [Key]
      public int fileID { get; set; }
      public Int16 stageId { get; set; }
      [Key]
      public Int16 activityid { get; set; }
      public Int16 bookid { get; set; }
      public byte status { get; set; }=1;
      public string  @lock { get; set; } ="N";
    }
}