using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using api.Courses;
using api.Fees;
using System.Globalization;

namespace api.Storage {
    /// <summary>
    /// Class for getting info from the internet
    /// Used to hold course listing information
    /// </summary>

    public class BufferedWebDB : IWebDB {
        // Web Client
        private readonly HttpClient client = new();

        // Timespan to hold cached data
        private readonly TimeSpan MaxCacheAge = TimeSpan.FromMinutes(5);

        // Dict of key=department,value=course listing
        private static readonly Dictionary<string,CachedCourseList> courseCache = new();
        

        public List<api.Courses.CourseInfo>  GetCoursesForDepartment(string Department){
            if(courseCache.ContainsKey(Department) && courseCache[Department].Age < MaxCacheAge){
                return courseCache[Department].CourseListing;
            }
            var newData = ScrapeForCourses(Department);
            courseCache[Department] = new CachedCourseList(){
                CourseListing = newData,
                LastModified = DateTime.Now,
            };
            return newData;
            
        }
        // Scrape catalog.uah.edu for course info
        private  List<api.Courses.CourseInfo> ScrapeForCourses(string Department){
            
            // Fetch both types of classes at the same time
            var underGrad = GetCourseListFromHttp($"https://catalog.uah.edu/undergrad/course-descriptions/{Department}/");
            var grad = GetCourseListFromHttp($"https://catalog.uah.edu/grad/course-descriptions/{Department}/");

            try {
                // wait for that to finish
               Task.WaitAll(underGrad,grad);
            }catch(AggregateException e){
                foreach(var exception in e.InnerExceptions){
                    Console.WriteLine(exception);
                }
            }
            
            List<api.Courses.CourseInfo> courseList = new();

            // If the tasks finished without error, import their data
            if(underGrad.IsCompletedSuccessfully){
                courseList.AddRange(underGrad.Result);
            }
            if(grad.IsCompletedSuccessfully){
                courseList.AddRange(grad.Result);
            }

            return courseList;
        }

        // Get a list of courses from the UAH Website.
        private async Task<List<api.Courses.CourseInfo>> GetCourseListFromHttp(string uri){
            HttpResponseMessage httpResponse = await client.GetAsync(uri);
            var courses = new List<api.Courses.CourseInfo>();            
            if(httpResponse.StatusCode == HttpStatusCode.OK){
                // Get the string of the body of the http response
                var body = await httpResponse.Content.ReadAsStringAsync();
                var blockIndicator = "<div class=\"courseblock\">";

                var cursor = body.IndexOf(blockIndicator);
                var divBlocks = new List<string>();

                while(cursor != -1 && body.Length > 0){
                    body = body[(cursor+blockIndicator.Length)..];
                    cursor = body.IndexOf("</div>");
                    divBlocks.Add(body[..cursor]);
                    cursor = body.IndexOf(blockIndicator);
                }
                
                var ParseDivBlock = (string block)=>{
                    var items = block.Split("<p").Where(p=>p.Contains('p'));
                    var keys = items.Select((item)=>{
                            // Grab the CSS class of the paragraph section
                            var itemList = item.ToList();
                            var start = item.IndexOf("\"")+1;
                            var end = item.LastIndexOf("\"");
                            item = new string(itemList.GetRange(start,end-start).ToArray());
                            return item.Replace("courseblock","");
                    });
                    var values = items.Select((item)=>{
                            var itemList = item.ToList();
                            var start = item.IndexOf(">")+1;
                            var end = item.LastIndexOf("<");
                            item = new string(itemList.GetRange(start,end-start).ToArray());
                            return item.Replace("<br/>","").Replace("\xa0"," ").Replace("&amp;","&").Replace("\n"," ");
                    });
                    return keys.Zip(values,(k,v) => new {k,v}).ToDictionary(x=>x.k, x=>x.v);

                };


                // Now that divBlocks is populated, parse that
                // Each div block represents a course for the subject 
                foreach(string block in divBlocks){
                    CourseInfo info = new();
                    var blockInfo = ParseDivBlock(block);
                    // Title is in the format:
                    // Department Number - Long Course Title
                    var title = blockInfo["title"];
                    var titleSegments = title.Split(" - ");
                    if(titleSegments.Length > 1){
                        // Make the long course title the top of the description
                        info.Description = titleSegments[1] + "\n";
                        // Parse the Department and Number out
                        titleSegments = titleSegments[0].Split(" ");
                        if(titleSegments.Length > 1){
                            info.Department = titleSegments[0];
                            info.Number = titleSegments[1];
                        }
                    }
                    // Credit Hours
                    var credits = blockInfo["credits"];
                    var creditSegments = credits.Split(": ");
                    if(creditSegments.Length > 1){
                        if (int.TryParse(creditSegments[1], out int creditHours))
                        {
                            info.CreditHours = creditHours;
                        }
                    }

                    // Description, which may or may not be there
                    if(blockInfo.ContainsKey("desc")){
                        info.Description += blockInfo["desc"];

                    }
                    courses.Add(info);

                }
                

            }
            return courses;
            
        }

        /// <summary>
        /// Represents a cached list of courses
        /// </summary>
        private class CachedCourseList {
            public List<api.Courses.CourseInfo> CourseListing = new();
            
            public DateTime LastModified {get;set;} = DateTime.MinValue;
            public TimeSpan Age{
                get {
                    return DateTime.Now - LastModified;
                } 
            }   
        }


    }

}