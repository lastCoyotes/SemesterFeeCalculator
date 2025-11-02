// Information Required to uniquely identify a course fee and costs
namespace api.Fees {
    
    public class Course {
        public string Department { get; set; }
        public string Number { get; set; }
        public decimal Fee { get; set; }
    }

}