using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// API Endpoint for Fees
namespace api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CourseController : ControllerBase
    {

        private readonly ILogger<CourseController> _logger;

        private readonly api.Services.ICourseService CourseService;

        public CourseController(ILogger<CourseController> logger)
        {
            api.Storage.IWebDB db = new api.Storage.BufferedWebDB();
            api.Storage.ExcelDB ExcelDB = api.Storage.ExcelDB.GetDatabase();
            CourseService = new api.Services.CourseService(db, ExcelDB);
            _logger = logger;
            _logger.LogDebug("Initialized Course Controller");
        }

        [HttpGet]
        [Route("/[controller]")]
        public IEnumerable<Courses.CourseInfo> Get(){
                return CourseService.CourseListing();
        }


       [HttpGet]
       [Route("/[controller]/{dep}")]
       public IEnumerable<Courses.CourseInfo> Get(string dep){
            return CourseService.CoursesForDepartment(dep);
       }

       [HttpGet]
       [Route("Departments")]
       public IEnumerable<string> GetDep(){
            return CourseService.GetDepartments();
       }
    }
}
