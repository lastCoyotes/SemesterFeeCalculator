// Define a custom component for a searchable dropdown
class combobox extends HTMLElement {
    // Load element into the Shadow DOM
    async connectedCallback(){
        // Grab the Raw HTML of the component
        const shadow = this.attachShadow({mode: "open"});
        let resp = await fetch("/ui/combobox/combobox.htm");
        let doc = await resp.text();
        shadow.innerHTML = doc;        
        // Add the CSS styles specific to this component
        let css = document.createElement('link');
        css.href = "/ui/combobox/combobox.css";
        css.rel="stylesheet";
        shadow.appendChild(css);

        // Get the text box for search
        let text = shadow.getElementById('menu');
        // Open/Close Dropdown
        text.onfocus = ()=> {this.openDropdown(),this.search()};
        text.onblur = ()=> this.closeDropdown();
        // Make the button next to the dropdown work
        let menuBtn = shadow.getElementById("menuBtn");
        menuBtn.onclick = ()=>{text.focus()};
        // JS has a lot of input variations
        text.onkeyup = text.onpaste = text.oninput = ()=> this.search();

        text.addEventListener("keydown",()=>{})
        text.onkeydown = (eve)=>{
            this.handleKeyPress(eve.key);
        }
    }

    // Specifies in keyboard control mode
    keyboardMode = false;
    lastText = "";
    handleKeyPress(key){
        let text = this.shadowRoot.getElementById("menu");
        if(key == "Enter"){
            if(this.selected != null)   {
                this.closeDropdown();
                text.blur();
            }
        } else if(key == "ArrowDown"){
            this.keyboardMode = true;
            if(this.selected == null){
                let slot = this.shadowRoot.getElementById("options").children[0];
                this.selectItem(slot.assignedElements()[0])
            }
            // Allow up and down movement
            let value = text.value;
            text.value = this.lastText;
            let next = this.nextDropDownElement(this.selected);
            if(next != null){
                this.selectItem(next);
            } else {
                text.value = value;
            }
        } else if(key == "ArrowUp"){
            this.keyboardMode = true;
            if(this.selected == null){
                let slot = this.shadowRoot.getElementById("options").children[0];
                this.selectItem(slot.assignedElements()[slot.assignedElements().length])
            }
            // Allow up and down movement
            let prev = this.previousDropDownElement(this.selected);
            if(prev != null){
                this.selectItem(prev);
            }
        } else {
            this.keyboardMode = false;
        }
    }
    nextDropDownElement(current){
        let slot = this.shadowRoot.getElementById("options").children[0];
        // Default to first element of the dropdown
        if(current == null){
            this.selectItem(slot.assignedElements()[0])
        }
        // Find the next available sibling
        let head = current;
        do{
            head = head.nextElementSibling;
            // Wrap around, but stop if there's nothing there
            if(head == null){
                head = slot.assignedElements()[0];
            }
        }while(head != null && head.hidden);
        // Return the next in line
        return head;
    }
    previousDropDownElement(current){
        let slot = this.shadowRoot.getElementById("options").children[0];
        // Default to first element of the dropdown
        if(current == null){
            this.selectItem(slot.assignedElements()[slot.assignedElements().length-1]);
        }   
        let head = current;
        // Find the previous available sibling
        do{ 
            head = head.previousElementSibling;
            if(head == null){
                head = slot.assignedElements()[slot.assignedElements().length-1];
            }
        }while(head != null && head.hidden);
        // Return the previous in line
        return head;
    }


    // Event that fires whenever the selected element changes
    onchange = null;



    // Show the dropdown
    openDropdown(){
        let dropdown = this.shadowRoot.getElementById("options");
        dropdown.classList.add("show");
        let text = this.shadowRoot.getElementById("menu");
        text.value = "";
    }
    
    // Hide the dropdown
    closeDropdown(){
        let dropdown = this.shadowRoot.getElementById("options")
        dropdown.classList.remove("show")
        
        let text = this.shadowRoot.getElementById('menu');
        if(this.selected == null){
            text.value = "";
        }else{
            text.value = this.selected.innerText;
        }
        // Send update since the dropdown value has changed
        if(this.onchange){
            this.keyboardMode = false;  
            this.onchange();
        }
    }
    

    // Chosen options
    selected = null;
    selectItem(item){
        if(this.selected != null){
            this.selected.classList.remove("selected");
        }
        this.selected = item;
        if(this.selected != null){
            this.selected.classList.add("selected");

        }   
        if(item == null){
            this.shadowRoot.getElementById("menu").value = "";
        }else {
            this.shadowRoot.getElementById("menu").value = item.innerText;
        }
    }

    // Find the element in the dropdown that's the closest match
    search(){
        if(this.keyboardMode){
            return; // Do nothing on keyboard selection
        }
        let text = this.shadowRoot.getElementById("menu");
        let options = this.shadowRoot.getElementById("options");
        let slot = options.children[0];
        // Find elements that match the search term
        slot.assignedElements().forEach(elem =>{
            const isVisible = elem.innerText.toLowerCase().includes(text.value.toLowerCase());
            elem.hidden = !isVisible;
            let triggerFunc = ()=>{ this.selectItem(elem); };
            elem.onclick = triggerFunc;
            elem.onmousedown = triggerFunc;
            /* This event causes more headaches than help
            elem.onmouseenter = triggerFunc;*/
        }); 
        
    }
    

}
customElements.define("combo-box",combobox);