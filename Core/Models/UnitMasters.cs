using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class UnitMasters
    {
        [Key] public string ID { get; set; }

        public string UnitID { get; set; }
        public string MasterID { get; set; }
    }
}