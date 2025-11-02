using System.Collections.Generic;

namespace api.Services {
    public interface ICourseService {
        public List<Courses.CourseInfo> CourseListing();
        public List<Courses.CourseInfo> CoursesForDepartment(string department);
        public List<string> GetDepartments();
    }
}