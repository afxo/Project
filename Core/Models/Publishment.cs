using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class Publishment
    {
        [Key] public string ID { get; set; }

        [Required] public string type { get; set; }

        [Required] public string title { get; set; }

        [Required] public string year { get; set; }

        public string uploadedby { get; set; }

        public string unit_that_belongs { get; set; }
    }
}