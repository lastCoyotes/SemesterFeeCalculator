// Production API Endpoint
const API_ROOT = '/api';

class Fee {
    // Get a list of Fees per college
    static async CollegeFees(){
        let resp = await fetch(API_ROOT+'/Fee/College');
        if(resp.status == 200){
            return await resp.json();   
        }
        console.log(resp);
        return [];
    }
    // Get the itemized Fee schedule for a set of courses.
    // The format of courses is one object formatted as:
    //  {"Course Name": CreditHrs}
    static async GetItemizedFeeSchedule(courses){

        let schedule = [];
        Object.keys(courses).forEach(course=>{
            schedule.push({
                "courseName" : course,
                "courseHours" : courses[course]
            })
        })

        let resp = await fetch(API_ROOT+"/Fee/Calculate",{
            method: "POST",
            headers: {
                'Content-Type':'application/json',
            },
            body: JSON.stringify(schedule),
        })
        if(resp.status == 200){
            return await resp.json();
        }
        return {};
    }

}