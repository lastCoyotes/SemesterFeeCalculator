using System.Collections.Generic;

namespace api.Storage {

    public interface IWebDB {

        /// <summary>
        /// Retrive Course Information for all courses given a department.
        /// </summary>
        public List<api.Courses.CourseInfo>  GetCoursesForDepartment(string Department);
    }

}