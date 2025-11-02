namespace tests.Controllers;

using api.Controllers;

[TestClass]
public class FeeControllerTest
{

    public static void SetTestEnvironment(){
        if (!File.Exists("UAHFeeSchedule.xlsx")){
            Environment.CurrentDirectory += "/../../../../../api/";
        }
    }
    // This is a sample test for an api endpoint
    [TestMethod]
    public void FeeController()
    {
        // Move up to project root
        SetTestEnvironment();
        var ctrl = new FeeController(null);
        var results = ctrl.Get();
        Assert.AreEqual(0, results.Count());
    }

    [TestMethod]
    public void TestCalculateAPI(){
       SetTestEnvironment();
       var ctrl = new FeeController(null);
        List<ScheduledCourse> testList = new()
        {
            new ScheduledCourse("CE 121", 5)
        };
        var results = ctrl.GetFeeForCourse(testList);
       Assert.IsInstanceOfType(results, typeof(Dictionary<string,Dictionary<string,decimal>>));
    }
    

    [TestMethod]
    public void TestGetSemesterAPI(){
       SetTestEnvironment();
       var ctrl = new FeeController(null);
       var results = ctrl.GetSemesterFeeValues();
       Assert.IsInstanceOfType(results, typeof(List<api.Storage.MiscFee>));
    }

    [TestMethod]
    public void TestGetOneTimeAPI(){
       SetTestEnvironment();
       var ctrl = new FeeController(null);
       var results = ctrl.GetOneTimeFeeValues();
       Assert.IsInstanceOfType(results, typeof(List<api.Storage.MiscFee>));
    }
}