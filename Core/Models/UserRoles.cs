using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class UserRoles
    {
        [Key] public string ID { get; set; }

        public string username { get; set; }
        public string role { get; set; }
    }
}