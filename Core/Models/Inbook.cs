using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class Inbook
    {
        [Key] public string ID { get; set; }

        [Required] public string givenID { get; set; }

        [Required] public string publisher { get; set; }

        [Required] public string chapter { get; set; }

        public string volume { get; set; }
        public string ISBN { get; set; }
        public string ISSN { get; set; }
    }
}