using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class Writer
    {
        [Key] public string ID { get; set; }

        [Required] public string name { get; set; }

        [Required] public string surrname { get; set; }

        public string orchidURL { get; set; }
        public string privateURL { get; set; }
        public string property { get; set; }
    }
}