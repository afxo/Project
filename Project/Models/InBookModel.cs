namespace Project.Models
{
    public class InBookModel
    {
        public string author { get; set; }
        public string title { get; set; }
        public string issn { get; set; }
        public string publisher { get; set; }
        public string year { get; set; }
        public string volume { get; set; }
        public string isbn { get; set; }
        public string chapter { get; internal set; }
    }
}