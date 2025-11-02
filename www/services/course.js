// Production API Endpoint
//const API_ROOT = '/api/';

class Course {
    static async GetCoursesDept(choice){
        let resp = await fetch(API_ROOT+`/Course/${choice}`);
        if(resp.status == 200){
            return await resp.json();   
        }
        return resp;
    }
 }
