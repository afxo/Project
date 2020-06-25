using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class Inproceeding
    {
        [Key] public string ID { get; set; }

        [Required] public string givenID { get; set; }

        [Required] public string booktitle { get; set; }

        public string editor { get; set; }

        public string pages { get; set; }
        public string address { get; set; }
        public string publisher { get; set; }
    }
}