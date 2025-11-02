// File containing infrastructure to build a searchable table from JSON



class Table {

    // Remove Row from the table
    static productDelete(table,row) {
        var rowCount = table.dataset.row;
        rowCount = rowCount - 1;
        table.dataset.row = rowCount;
        let rm = row.parentElement.parentElement;
        rm.remove();
    }

    // Creates the button element that removes a course
    static newRemoveButton(table){
        let createButton = document.createElement('button');
        createButton.setAttribute("type", 'button'); 
        createButton.onclick = ()=> {this.productDelete(table, createButton);}; // for IE
        createButton.innerText = " X ";
        createButton.classList.add("btn");
        createButton.classList.add('rmButton'); 
        return createButton;
    }
    
    // Fill a table with data
    // Args:
    //      table => element to populate
    //      array => list of objects, one per row
    //      headers => array of fields to extract from the objects
    static PopulateTable(table,array,headers) {
        // Get the table element to insert data into
        //var table = document.getElementById(id);
        table.innerHTML = null;
        let row =0,col = 0;
        // Insert Table Headers
        let tr = document.createElement('tr');
        tr.dataset.row = row;
        headers.forEach(header => {
            // Create header
            let th = document.createElement('th');
            th.dataset.row = row;
            th.dataset.column = col;
            let text = header ;//.replace('_',' ');
            //text = text.charAt(0).toUpperCase() + text.substr(1).toLowerCase();
            th.innerText = text;
            // Add header to table
            tr.appendChild(th);
            col++;
        });
        table.appendChild(tr);
        row++;
        // Internal Function to add element
        var populateFunc = (element)=>{
            col = 0;
            let tr = document.createElement('tr');
            tr.dataset.row = row;
            let values = [];
            headers.forEach(header =>{
                //values.push()
                let td = document.createElement('td');
                td.dataset.row = row;
                td.dataset.column = col;

                if(header === "Remove Course"){
                    td.innerText = "";
                    td.appendChild(this.newRemoveButton(table));
                }else{
                    // Bad values turn into 0.
                    td.innerText = (element[header] == undefined)? 0 : element[header]; 
                }
                
                tr.appendChild(td);
                col++;
                
                
            });
            table.appendChild(tr);
            row++;
    
            table.dataset.row = row;
            table.dataset.column = col;
        }
    
        // Table Data, if array use for each, if object iterate over keys
        if(Array.isArray(array)){
            array.forEach(populateFunc);
        } else {
            Object.keys(array).forEach((key)=>{
                    let obj = array[key];
                    obj["Course"] = key;
                    populateFunc(obj);
    
            });
        }


}
    // Returns an html node at the specified row and column
    static GetTableCell(table, row,col){
        //let table = document.getElementById(tableID);
        return table.children[row].children[col];
    }
    // Returns an entire column
    static GetTableColumn(table,col){
        //let table = document.getElementById(tableID);
        let column = [];
        for(let r = 0; r < table.dataset.row; r++){
            column.push(table.children[r].children[col]);
        }
        return column;
    }

    // Appends an array of values to table as a column
    static AppendTableColumn(table,values){
        //let table = document.getElementById(tableID);
        for(let r = 0; r < table.dataset.row; r++){
            let td;
            // The first row is headers
            if(r == 0){
                td = document.createElement("th");
            }else {
                td = document.createElement("td");
            }
            td.innerText = values[r];
            td.dataset.row = r;
            td.dataset.column = table.dataset.column;
            table.children[r].appendChild(td);     
        }
        table.dataset.column++;
    
    }
    // Appends an array of values as a row to the table
    static AppendTableRow(table,values){
        //let table = document.getElementById(tableID);
        let row = document.createElement('tr');
        for(let c = 0; c < table.dataset.column; c++){
            let td = document.createElement('td');
            if(values[c] == undefined){
                td.innerText = "";
                td.appendChild(this.newRemoveButton(table));

            }else{
                td.innerText = values[c];
            }
            //td.innerText = values[c];
            row.appendChild(td);
        }
        row.dataset.row = table.dataset.row;
        //table.append(newRemoveButton());
        table.dataset.row++;
        table.append(row);
    }

    // See if a value is in a specified column already
    // Args:
    //      table => element to query
    //      col => which column to check
    //      query => string to look for
    static QueryColumn(table,col,query,predicate=null){
        if(predicate == null){
            predicate = (query,cell)=>{return query == cell};
        }
        // Get all rows of the table
        let rows = table.getElementsByTagName("td");
        // If the table is empty, let the predicate's default value on null be true
        if(rows.length == 0){
            return predicate(query,null);
        }
        for(let row of rows){
            // If the column matches and the predicate matches, the value was found
            if(row.dataset.column == col && predicate(query,row.innerText)){
                   return true;
            }
        }
        return false;
    }

}