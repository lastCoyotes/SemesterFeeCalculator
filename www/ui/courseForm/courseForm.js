class CourseForm extends HTMLElement {
    
    constructor(){
        super();
    }

    async connectedCallback(){
        
        // Grab the html
        const shadow = this.attachShadow({mode:"open"});
        let resp = await fetch("/ui/courseForm/courseForm.htm");
        let doc = await resp.text();
        shadow.innerHTML = doc;
        // Grab the CSS
        let css = document.createElement('link');
        css.href = "/ui/courseForm/courseForm.css";
        css.rel="stylesheet";
        shadow.appendChild(css);
        // Fill the department dropdown
        this.PopulateDepartments();

        this.initAdditionalFeeForm();

        // Initialize the elements
        await this.initEvents();

    }

    // Set events on objects
    async initEvents(){
        // Update Course List when department changes
        let department = this.shadowRoot.getElementById("dept");
        department.onchange = ()=>{this.PopulateCourses()};
        // Update Credit Hour field when course changes
        let classDropDown = this.shadowRoot.getElementById("classDropDown");
        classDropDown.onchange = ()=>{this.UpdateHours()};
        // Add Course button
        this.shadowRoot.getElementById("addCourseBtn").onclick = ()=>{this.AddCourseToTable()};
        // Submit button
        this.shadowRoot.getElementById("submit").onclick = ()=>{this.Submit()};

        // Error Check Credit hours
        //let creditHrs = this.shadowRoot.getElementById("creditHourTextBox");
        

        // Panel Open/Close
        this.shadowRoot.getElementById("SemesterPanel").ontoggle = ()=>{
            let semesterTable = this.shadowRoot.getElementById("SemesterFeeTable");
            let semesterTotal = this.shadowRoot.getElementById("SemesterTotal");
            this.calculateSpecialFees(semesterTable,semesterTotal);
        }
        this.shadowRoot.getElementById("OneTimePanel").ontoggle = ()=>{
            let oneTimeTable = this.shadowRoot.getElementById("OneTimeFeeTable");
            let oneTimeTotal = this.shadowRoot.getElementById("OneTimeTotal");
            this.calculateSpecialFees(oneTimeTable,oneTimeTotal);
        }
    } 

    // One Time and Semester Fee Logic

    initAdditionalFeeForm(){
        // Semester Fees
        let semesterTable = this.shadowRoot.getElementById("SemesterFeeTable");
        let semesterTotal = this.shadowRoot.getElementById("SemesterTotal");
        let endpoint = "/api/Fee/Semester";
        let action = (table)=>{
            this.calculateSpecialFees(table,semesterTotal);
        }
        this.initSpecialFees(semesterTable,endpoint,action);

        // One Time Fees
        let oneTimeTable = this.shadowRoot.getElementById("OneTimeFeeTable");
        let oneTimeTotal = this.shadowRoot.getElementById("OneTimeTotal");
        endpoint = "/api/Fee/OneTime";
        action = (table)=>{
            this.calculateSpecialFees(table,oneTimeTotal);
        }
        this.initSpecialFees(oneTimeTable,endpoint,action)

    }

    async initSpecialFees(table,endpoint,action=(table)=>{}){
        let headers = ["Fee", "Amount", "Times Charged","Description"];

        let resp = await fetch(endpoint);
        let feeList = await resp.json();

        let fees = [];
        feeList.forEach(fee=>{
            fees.push({
                "Fee" : fee["feeName"],
                "Amount": fee["feeCost"],
                "Times Charged": "",
                "Description": fee["feeDescription"]
            })
        })
        Table.PopulateTable(table,fees,headers);    
        for(let r = 1; r < table.dataset.row; r++){
            let inp = document.createElement("input",{is: "number-picker"});
            inp.type = "number"
            inp.value = 0;
            inp.onblur = (event)=>{
                // Default element to modify
                inp.onblur();
                // Update Summary
                action(table);
            }
            inp.onchange = (event)=>{
                action(table);
            }
            

            /*
            inp.onchange = inp.onblur = ()=>{
                if(inp.value == "" || isNaN(Number(inp.value))){
                    inp.value = 0;
                }
                
                action(table);

            }
            */
            
            Table.GetTableCell(table,r,2).appendChild(inp);
        }
    }

    calculateSpecialFees(semesterTable,outputElement){
        let total = 0;
        for(let r = 1; r < semesterTable.dataset.row; r++){
            // Fee Cost
            let cost = Table.GetTableCell(semesterTable,r,1).innerText;
            // Times Incurred
            let multiple = Table.GetTableCell(semesterTable,r,2).firstChild.value;
            total += cost*multiple;
        }
        outputElement.innerText = "$" + total;
    }

    // Allow the external element to get some values
    getSpecialFeeTotal(){
        let semesterTotal = this.shadowRoot.getElementById("SemesterTotal").innerText;
        let oneTimeTotal = this.shadowRoot.getElementById("OneTimeTotal").innerText;
        return Number(semesterTotal.replace("$","")) + Number(oneTimeTotal.replace("$",""));
    }


    // Populate the combobox for departments
    async PopulateDepartments(){
            // TODO: Get Department List from the API
            let resp = await fetch("/api/Course/Departments");
            let departments = await resp.json();
            let departmentDropDown = this.shadowRoot.getElementById("dept");
            departmentDropDown.innerHTML = "";
            // Combobox Data
            departments.forEach(element => {
                let opt = document.createElement('li');
                opt.classList.add("dropdown-item");
                opt.dataset.value = element.toLowerCase();
                opt.innerText = element;
                departmentDropDown.appendChild(opt);
            });
              
    }

    // Populate the combobox for courses
    async PopulateCourses(){
        var departmentDropDown = this.shadowRoot.getElementById("dept");
        var classDropDown = this.shadowRoot.getElementById("classDropDown");
        //  classDropDown.innerHTML = "";
        while(classDropDown.firstChild){
            classDropDown.removeChild(classDropDown.firstChild);
        }
        
        if(classDropDown.selected != null){
            // Clear the select
            classDropDown.selectItem(null);
        }

        var department = departmentDropDown.selected.dataset.value;
        // Populate the class select
        let array = await Course.GetCoursesDept(department)
        if(array == null || !Array.isArray(array) ){
            alert("Failed to get Departments. Please try again later.");
            return;
        }
        // Combobox Data
        array.forEach(element => {
            let opt = document.createElement('li');
            opt.setAttribute("data-value", `${element.department + " " + element.number}`);
            opt.classList.add("dropdown-item");
            opt.dataset.credits = element.creditHours;
            let opttxt = document.createTextNode(`${element.department + " " + element.number}`);
            opt.appendChild(opttxt);
            classDropDown.appendChild(opt);
        });


    }
    
    // function for populating the hours text field
    async UpdateHours(){
        var classopt= this.shadowRoot.getElementById("classDropDown");
        var classopttxt = "";
        //classopttxt+=classopt.options[classopt.selectedIndex].value;// store the hours value from the class select
        if(classopt.selected != null){
            //classopttxt=classopt.selected.value;
            classopttxt=classopt.selected.dataset.credits
        }
        this.shadowRoot.getElementById("creditHourTextBox").value=classopttxt;         // display the value in the hours text field
    }

    // Gets the current selected course from the list
    GetCurrentSelectedCourse(){
        var classDropDown = this.shadowRoot.getElementById("classDropDown");
        var opt = classDropDown.selected;
        var hrs = Number(this.shadowRoot.getElementById("creditHourTextBox").value);
        //var btn = newRemoveButton();
        return {
            "Course Name": opt.dataset.value,
            "Credit Hours": hrs,
            //"Remove Course": btn
        }
    }

    // Adds course to table with id 
    AddCourseToTable(){
        
        let table = this.shadowRoot.getElementById("CourseTable");
        let data = this.GetCurrentSelectedCourse();
        if(Table.QueryColumn(table,0,data["Course Name"])){
            return; // The course has been added
            // TODO: Add nonintrusive msg for the user
        }
        if(table.children.length == 0){
            Table.PopulateTable(table,[data],["Course Name","Credit Hours", "Remove Course"]);
        }else {
            Table.AppendTableRow(table,[data["Course Name"],data["Credit Hours"], data["Remove Course"]]);
        }
    
        
    }    
    

    // Gets the courses from the table
    GetCourseTableData(){
        let tableID = "CourseTable";
        let table = this.shadowRoot.getElementById(tableID);
        let data = {};
        for(let r = 1; r < table.dataset.row; r++){
            console.log(Table.GetTableCell(table,r,0).innerText);
            data[Table.GetTableCell(table,r,0).innerText] = Table.GetTableCell(table,r,1).innerText;
        }
        return data;   
    }
    getTableRowCount(){
        let table = this.shadowRoot.getElementById("CourseTable");
        Table.productDelete(table, table.children[row]);
        Table.newRemoveButton(table);
    }

    // Submits the data in the form, updates the grand total
    async Submit(){
        if(this.onsubmit != null){
            this.onsubmit(this);
        }
        return;
    }
}
customElements.define("course-form",CourseForm);