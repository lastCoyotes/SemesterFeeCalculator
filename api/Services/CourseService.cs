using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using System;
using System.Text.RegularExpressions;

namespace api.Services
{
    public class CourseService : ICourseService {

        private readonly api.Storage.IWebDB storage;

        public api.Storage.ExcelDB ExcelDB;

        public CourseService(api.Storage.IWebDB db, api.Storage.IDatabase ExcelDatabase){
            storage = db;
            ExcelDB = (api.Storage.ExcelDB) ExcelDatabase;
        }

        /// <summary>
        /// Retrieves courses for a single department.
        /// </summary>
        /// <param name="department">The department to retrieve courses for</param>
        public List<Courses.CourseInfo> CoursesForDepartment(string department){
            // TODO Validate this this department string more strictly.
            if(Regex.IsMatch(department,"^[a-z]{2,4}$")){
                return storage.GetCoursesForDepartment(department);
            }
            throw new ArgumentException("Invalid Department");
        }

        public List<Courses.CourseInfo> CourseListing(){
            // TODO: Source this from somewhere
            string[] departments = new string[]{
                "cs","cpe"
            };

            var courseList = new List<Courses.CourseInfo>();            
            foreach(var dep in departments){
                var depList = storage.GetCoursesForDepartment(dep);
                courseList.AddRange(depList);
            }
            return courseList;
        }

        public List<string> GetDepartments(){
            List<string> departments = new();
            foreach(KeyValuePair<string, api.Storage.Department> department in ExcelDB.DepartmentSheet){
                departments.Add(department.Key);
            }
            return departments;
        }

    }   
}