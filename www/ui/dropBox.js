// File containing infrastructure to populate a the combobox from JSON


// Args:
    // id = combobox element to populate
function PopulateDropBox(id,array) {
    var select = document.getElementById(id);
    // init the first option to be shown in the select box
    //select.innerHTML = '"<option value="3" selected="select">select</option>"';
    // make the select box editable
    select.disabled = false;
    // Wipe out children
    select.innerHTML = "";
    select.childNodes.forEach(n => select.removeChild(n));

    if(array == null){
        return;
    }
    // select Data
    array.forEach(element => {
            let opt = document.createElement('li');
            opt.setAttribute("value", `${element.department + " " + element.number}`);
            opt.classList.add("dropdown-item")
            opt.dataset.credits = element.creditHours;
            let opttxt = document.createTextNode(`${element.department + " " + element.number}`);
            opt.appendChild(opttxt);
            select.appendChild(opt);
    });

}