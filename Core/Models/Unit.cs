using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class Unit
    {
        [Key] public string ID { get; set; }

        [Required] public string name { get; set; }

        [Required] public string type { get; set; }

        [Required] public string institute { get; set; }

        public string description { get; set; }

        [Required] public string url { get; set; }
    }
}