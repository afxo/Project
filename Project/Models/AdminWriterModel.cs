using System.Collections.Generic;
using Core.Models;

namespace Project.Models
{
    public class AdminWriterModel
    {
        public List<Writer> writer { get; set; }
        public Dictionary<string, int> counter { get; set; }
    }
}