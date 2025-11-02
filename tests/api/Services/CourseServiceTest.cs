namespace tests.Storage;
using System.Collections;
using api.Storage;

[TestClass]
public class CourseServiceTest{
    // Set location of excel file relative to current test, default to the one the application uses
    public static void SetTestEnvironment(){
        if (!File.Exists("UAHFeeSchedule.xlsx")){
            Environment.CurrentDirectory += "/../../../../../api/";
        }
    }
    // Stub for setting up database
    public class MockIWebDB:IWebDB{
          public List<api.Courses.CourseInfo>  GetCoursesForDepartment(string Department){
            return new List<api.Courses.CourseInfo>();
          }
    }

    public class MockExcelDB:IDatabase{
        public void PopulateDepartmentSheet(ArrayList departmentRow){}
        // Populate department fees from department sheet
        public void PopulateCollegeSheet(ArrayList collegeRow){}

        // Populate individual fees from individual fees sheet
        public void PopulateIndCourseSheet(ArrayList indCourseRow){}
        // Populate group course fees from group course fees sheet
        public void PopulateGrpCourseSheet(ArrayList grpCourseRow){}
        // Populate semester fees from semester fee sheet
        public void PopulateSemesterFeesSheet(ArrayList semesterFeesRow){}
        // Populate one time fees from one time fee sheet
        public void PopulateOneTimeFeesSheet(ArrayList oneTimeFeesRow){}
    }

    // Tests getting courses for a given department
    // True means the department could be valid
    [DataTestMethod]
    [DataRow("cs", true)]
    [DataRow("cpe", true)]
    [DataRow("a", false)]
    [DataRow("aaaaa", false)]
    [DataRow("11", false)]
    [DataRow("", false)]
    [DataRow("  ", false)]
    public void TestCourseForDepartment(string department, bool expected){
        SetTestEnvironment();
        api.Services.CourseService cs = new(new MockIWebDB(), ExcelDB.GetDatabase());
        bool threw = true;
        try 
        {
            cs.CoursesForDepartment(department);
        }
        catch(ArgumentException)
        {
            threw = false;
        }
        Assert.AreEqual(expected, threw, $"string: {department}");
    }
}
        

