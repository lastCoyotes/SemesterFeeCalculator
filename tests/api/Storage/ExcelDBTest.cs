namespace tests.Storage;

using api.Storage;

[TestClass]
public class ExcelDBTest{

    public static void SetTestEnvironment(){
        if (!File.Exists("UAHFeeSchedule.xlsx")){
            Environment.CurrentDirectory += "/../../../../../api/";
        }
    }
    // Tests Excel functions
    [TestMethod]
    public void ExcelInterop(){
        SetTestEnvironment();
        api.Storage.IDatabase db = api.Storage.ExcelDB.GetDatabase(); 
        Assert.IsNotNull(db);
    }

    [DataTestMethod]
    [DataRow(1,2)]
    public void ExcelTest(int a, int b){
        Assert.AreEqual(a+b, 3);
    }

}

