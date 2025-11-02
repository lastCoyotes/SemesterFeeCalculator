namespace tests.Storage;
using System.Collections.Generic;
using api.Storage;

[TestClass]
public class FeeServiceTest{
    // Ensures an excel file is present
    public static void SetTestEnvironment(){
        if (!File.Exists("UAHFeeSchedule.xlsx")){
            Environment.CurrentDirectory += "/../../../../../api/";
        }
    }
    public class MockIWebDB:IWebDB{
          public List<api.Courses.CourseInfo> GetCoursesForDepartment(string Department){
            return new List<api.Courses.CourseInfo>();
          }
    } 

    
    [DataTestMethod]
    public void TestGetCourseFees(){
        SetTestEnvironment();
        api.Services.FeeService fs = new (ExcelDB.GetDatabase());
        var testDict1 = fs.GetCourseFees("CS 113", 1);
        var testDict2 = fs.GetCourseFees("MU 106", 1);
        var testDict3 = fs.GetCourseFees("NUR 623", 1);

        Dictionary<string,decimal> dictResult1 = new(){{"Department Fee", 0}, {"College Fee", 62}};
        Dictionary<string,decimal> dictResult2 = new(){{"Department Fee", 125}, {"College Fee", 46}, {"Group Course Fees", 10}};
        Dictionary<string,decimal> dictResult3 = new(){{"Department Fee", 0}, {"College Fee", 68}, {"Individual Course Fees", 575}};

        CollectionAssert.AreEqual(
        testDict1.OrderBy(kv => kv.Key).ToList(),
        dictResult1.OrderBy(kv => kv.Key).ToList()
        );
        CollectionAssert.AreEqual(
        testDict2.OrderBy(kv => kv.Key).ToList(),
        dictResult2.OrderBy(kv => kv.Key).ToList()
        );
        CollectionAssert.AreEqual(
        testDict3.OrderBy(kv => kv.Key).ToList(),
        dictResult3.OrderBy(kv => kv.Key).ToList()
        );
    }

    [DataRow("CPE", 1)]
    [DataRow("113", 1)]
    [DataRow("",1)]
    [DataTestMethod]
    public void TestInvalidGetCourseFees(string course, int hours){
        SetTestEnvironment();
        api.Services.FeeService fs = new (ExcelDB.GetDatabase());
        var testDict = fs.GetCourseFees(course, hours);
        Assert.IsFalse(testDict.Any());
    }

    [DataTestMethod]
    public void TestGetSemesterFees(){
        SetTestEnvironment();
        Dictionary<string, int> semesterDict = new(){
        {"Honors College", 1},
        {"International (Academic Year)", 1},
        {"International Fee(5 week term)", 1},
        {"International Fee(10 week term)", 1},
        {"Parking Permit (Full Year)", 1},
        {"Parking Permit (Spring & Summer)", 1},
        {"Parking Permit (Summer Only)", 1},
        {"Meal Plan Options 1-4", 1},
        {"Meal Plan Option 5", 1},
        {"Meal Plan Option 6", 1},
        {"Meal Plan Options 7-8", 1}
        
        };
        api.Services.FeeService fs = new (ExcelDB.GetDatabase());
        var semesterFees = fs.GetSemesterFees(semesterDict);
        Assert.IsInstanceOfType(semesterFees, typeof(Dictionary<string,decimal>));
    }

      [DataTestMethod]
    public void TestGetOneTimeFees(){
        SetTestEnvironment();
        Dictionary<string, int> oneTimeDict = new(){
        {"J Visa", 1},
        {"Academic Transcript", 1},
        {"Credit by Departmental Exam", 1},
        {"Charger ID (New Students)", 1},
        {"Charger ID (Replacement)", 1}
};
        api.Services.FeeService fs = new (ExcelDB.GetDatabase());
        var oneTimeFees = fs.GetOneTimeFees(oneTimeDict);
        Assert.IsInstanceOfType(oneTimeFees, typeof(Dictionary<string,decimal>));
    }
 
    [DataTestMethod]
     public void TestInvalidGetOneTimeFees(){
        SetTestEnvironment();
        Dictionary<string, int> oneTimeDict = new(){
        {"J",1}};

        api.Services.FeeService fs = new (ExcelDB.GetDatabase());
        var oneTimeFees = fs.GetOneTimeFees(oneTimeDict);
        Assert.IsFalse(oneTimeFees.Any());
    }

    [DataTestMethod]
     public void TestInvalidGetSemesterFees(){
        SetTestEnvironment();
        Dictionary<string, int> semesterDict = new(){
        {"Honors",1}};

        
        api.Services.FeeService fs = new (ExcelDB.GetDatabase());
        var semesterFees = fs.GetSemesterFees(semesterDict);
        Assert.IsFalse(semesterFees.Any());
    }
}





