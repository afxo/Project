using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class Book
    {
        [Key] public string ID { get; set; }

        public string givenID { get; set; }

        [Required] public string publisher { get; set; }

        public string volume { get; set; }
        public int number { get; set; }
        public string series { get; set; }
        public string address { get; set; }
        public string edition { get; set; }
        public string ISBN { get; set; }
        public string ISSN { get; set; }
    }
}