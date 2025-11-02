using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// API Endpoint for Fees
namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FeeController : ControllerBase
    {

        private readonly ILogger<FeeController> _logger;

        private readonly api.Services.IFeeService FeeService;

        public FeeController(ILogger<FeeController> logger)
        {
            _logger = logger;
            _logger?.LogInformation("Initializing Database");
            api.Storage.IDatabase db = api.Storage.ExcelDB.GetDatabase();
            FeeService = new api.Services.FeeService(db);
        }

        // Unused API
        [HttpGet]
        public IEnumerable<Fees.Course> Get()
        {

            return new List<Fees.Course>();
        }

        // Unused API
        [HttpGet]
        [Route("College")]
        public IEnumerable<Fees.Colleges> Colleges()
        {

            //var rng = new Random();
            return new List<Fees.Colleges>(){
                new Fees.Colleges(){
                    Department = "AMS",
                    DepartmentFee = 0,
                    College = "Arts, Humanities, Social Sciences",
                    CollegeFee = 24
                }
            };
        }

        /* Post request that takes as input a list of courses and returns a dictionary with 
           the key as course name and the value as a dictionary of the separate course fees.
           */
        [HttpPost]
        [Route("Calculate")]
        public Dictionary<string, Dictionary<string, Decimal>> GetFeeForCourse([FromBody] List<ScheduledCourse> courseList)
        {
            Dictionary<string, Dictionary<string, Decimal>> calculatedFees = new();
            foreach (ScheduledCourse entry in courseList)
            {
                calculatedFees[entry.CourseName] = FeeService.GetCourseFees(entry.CourseName, entry.CourseHours);
            }
            return calculatedFees;
        }

        /* Post request that takes as input an object with various semester fees and returns
            a dictionary mapping each fee to its numerical value.
           */
        [HttpPost]
        [Route("Semester")]
        public Dictionary<string, Decimal> GetSemesterFees([FromBody] Dictionary<string,int> semesterFees)
        {
            return FeeService.GetSemesterFees(semesterFees);
        }

        /* Post request that takes as input an object with various one time fees and returns
            a dictionary mapping each fee to its numerical value.
           */
        [HttpPost]
        [Route("OneTime")]
        public Dictionary<string, Decimal> GetOneTimeFees([FromBody] Dictionary<string,int> oneTimeFees)
        {
            return FeeService.GetOneTimeFees(oneTimeFees);
        }

        /*
        Get request to return the list of one time fees and their values.
        */
        [HttpGet]
        [Route("OneTime")]
        public List<api.Storage.MiscFee> GetOneTimeFeeValues()
        {
            return FeeService.GetOneTimeFeeValues();
        }

        /*
        Get request to return the list of semester fees and their values.
        */
        [HttpGet]
        [Route("Semester")]
        public List<api.Storage.MiscFee> GetSemesterFeeValues()
        {
            return FeeService.GetSemesterFeeValues();
        }

        /*
        Get request to return the list of departments.
        */
        
    }

    // Course a user scheduled
    public class ScheduledCourse {
        public string CourseName {get; set;}
        public int CourseHours {get; set;} 

        public ScheduledCourse(){}

        public ScheduledCourse(string name, int hours){
            CourseName = name;
            CourseHours = hours;
        }
}
}
