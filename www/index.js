// Calculates the Final Cost of Fees
function CalcGrandTotal(){           
    // Calculate the cost of taking the courses
    let total = 0;
    // Ensure calculation occurs
    let courseFees = document.getElementById("ItemizedFeeTable");
    // Some error occurred with course Fees
    if(courseFees == null || courseFees.lastChild == null || courseFees.lastChild.lastChild == null){
        total = 0;
    }else {
        total += Number(courseFees.lastChild.lastChild.innerText);
    }
    // Total was invalid
    if(isNaN(total)){
        total = 0;
    }

    document.getElementById("CourseTotal").innerText = "$" + String(total);
    // Add the other costs
    let form = document.getElementById("form");
    total += form.getSpecialFeeTotal()
    document.getElementById("GrandTotal").innerText = "$" +  String(total);
}   
// Get the Itemized List of Fees
async function SubmitForm(){
    form = document.getElementById("form");
    //let courses = {"AST 106":4,"MA 244":3,"CS 121":3,"ARS 160":3};
    let courses = form.GetCourseTableData()
    let tbl = await Fee.GetItemizedFeeSchedule(courses);
    let table = document.getElementById("ItemizedFeeTable");
    let headers = ["Course","Department Fee","College Fee","Individual Course Fees","Group Course Fees"];
    Table.PopulateTable(table,tbl,headers);
    try {
        // Total up the rows
        let rowSums = [];
        rowSums.push("Totals");
        for(let r = 1; r < table.dataset.row; r++){
            let rowTotal = 0;
            for(let c = 1; c < table.dataset.column; c++){
                let tmp = Number(Table.GetTableCell(table,r,c).innerText);
                rowTotal += tmp
            }
            rowSums.push(rowTotal);
        }
        // Add the totals of the rows as a column        
        Table.AppendTableColumn(table,rowSums);
        // Total up the columns
        let colTotals = [];
        colTotals.push("Totals")
        for(let c = 1; c < table.dataset.column; c++){
            let col = Table.GetTableColumn(table,c);
            let sum = 0;
            col.slice(1).forEach(element => {
                sum += (Number(element.innerText));
            });
            colTotals.push(sum);
        };
        Table.AppendTableRow(table,colTotals);
    }
    catch (error) {
       console.log(error);
    }
    CalcGrandTotal();
} 
