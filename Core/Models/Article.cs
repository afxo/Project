using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class Article
    {
        [Key] public string ID { get; set; }

        [Required] public string givenID { get; set; }

        [Required] public string mag_name { get; set; }

        public string volume { get; set; }
        public int page_num { get; set; }
        public string pages { get; set; }
        public string author { get; set; }
    }
}