
namespace api.Courses{
    ///<summary>
    /// Course on the UAH Catalog
    /// </summary>
    public class CourseInfo {
       public string Department {get;set;} = "";
       public string Number {get;set;} = "";
       public int CreditHours {get;set;} = 0;

       public string Description {get;set;}= "";
    }
}