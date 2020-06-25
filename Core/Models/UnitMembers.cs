using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class UnitMembers
    {
        [Key] public string ID { get; set; }

        public string Username { get; set; }
        public string ClassID { get; set; }
    }
}