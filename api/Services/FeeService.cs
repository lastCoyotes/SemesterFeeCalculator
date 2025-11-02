// Implemenation of Fee Service Interface

using api.Storage;
using System.Collections.Generic;
using System.Linq;
using System;

namespace api.Services
{
    public class FeeService : IFeeService
    {
        public ExcelDB DB;
        // A set which contains all departments with courses that have individual fees
        private readonly HashSet<string> acceptableIndCourseList;
        // A set which contains all departments with courses that have group fees
        private readonly HashSet<string> acceptableGroupCourseList;

        public FeeService(IDatabase db)
        {
            DB = (ExcelDB)db;
            acceptableIndCourseList = DB.IndividualCourses;
            acceptableGroupCourseList = DB.GroupCourses;
        }

        public Dictionary<string, decimal> GetCourseFees(string course, int hours)
        {
            Dictionary<string, decimal> finalCourseFees = new();
            CourseNumber currentCourse = new(course);
            
            //Check to see if the courses have individual or group fees 
            bool indCourseFee = CheckIndCourseFees(currentCourse.Department);
            bool grpCourseFees = CheckGrpCourseFees(currentCourse.Department);
    
            try
            {
                finalCourseFees["Department Fee"] = DB.DepartmentSheet[currentCourse.Department].GetDepartmentFee() * hours;
                finalCourseFees["College Fee"] = DB.CollegeSheet[DB.DepartmentSheet[currentCourse.Department].GetCollege()] * hours;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception " + e + " was thrown!");
            }

            if (indCourseFee)
            {
                if (DB.IndCourseSheet.ContainsKey(course))
                {
                    finalCourseFees["Individual Course Fees"] = DB.IndCourseSheet[course]*hours;
                }
            }

            if (grpCourseFees)
            {
                if (DB.GrpCourseFees.ContainsKey(course))
                {
                    finalCourseFees["Group Course Fees"] = DB.GrpCourseFees[course] * hours;
                }
            }

            return finalCourseFees;
        }

        public Dictionary<string,decimal> GetSemesterFees(Dictionary<string, int> semester) {
            Dictionary<string,decimal> semesterFees = new();

            foreach((string fee, int feeCount) in semester){
                if (feeCount>0){
                    try{
                semesterFees[fee] = DB.SemesterFees[fee].FeeCost*feeCount;
                    }
                    catch(KeyNotFoundException e){
                        Console.WriteLine("Exception in input: "+ e);
                        return semesterFees;
                    }
                }
            }

            return semesterFees;
        }


        public Dictionary<string,decimal> GetOneTimeFees(Dictionary<string,int> oneTime){
            Dictionary<string,decimal> oneTimeFees = new();

            foreach((string fee, int feeCount) in oneTime){
                if (feeCount>0){
                    try{
                    oneTimeFees[fee] = DB.OneTimeFees[fee].FeeCost*feeCount;
                    }
                    catch(KeyNotFoundException e){
                        Console.WriteLine("Exception in input: "+ e);
                        return oneTimeFees;
                        
                    }
                }
            }

            return oneTimeFees;
        }

        public List<MiscFee> GetOneTimeFeeValues(){
            return DB.OneTimeFees.Values.ToList();;
        }

        public List<MiscFee> GetSemesterFeeValues(){
            return DB.SemesterFees.Values.ToList();
        }

        public bool CheckIndCourseFees(string department)
        {
            return acceptableIndCourseList.Contains(department);
        }

        public bool CheckGrpCourseFees(string department)
        {
            return acceptableGroupCourseList.Contains(department);
        }
    };

}