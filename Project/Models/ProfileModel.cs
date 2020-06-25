using System.Collections.Generic;
using Core.Models;

namespace Project.Models
{
    public class ProfileModel
    {
        public string ID { get; set; }
        public string username { get; set; }
        public string membership { get; set; }
        public string insitute { get; set; }
        public string role { get; set; }
        public string writername { get; set; }
        public string writersurrname { get; set; }
        public string writerorchidurl { get; set; }
        public string writerprivateurl { get; set; }
        public string writerproperty { get; set; }

        public int count { get; set; } //Counts how many notifications the user have

        public List<Unit> avaliableUnits { get; set; }

        public List<string> choises { get; set; }
    }
}