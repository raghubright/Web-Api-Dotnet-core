using  System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovejobtoWms.Models
{
       
    [Table("WMS_mstBookDetails")]
    public class BookDetail
    {
        [Key]
      public short bookId { get; set; }
      
      public string  bookName { get; set; }
    }
}