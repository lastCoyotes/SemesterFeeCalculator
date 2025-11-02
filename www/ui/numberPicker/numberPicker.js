// Extension to the input element
class NumberPicker extends HTMLInputElement {
    constructor(){
        super();
        this.lastValue = this.value;
    }
    // Sets validation on number picker
    async connectedCallback(){
        this.type = "number";
        this.oninput = (event)=>{
            // Allow empties until blur
            if(event.data == null && this.value == ""){
                return;
            }
            // Block letters            
            // Sometimes JS will automatically wipe out the field if letters exist
            // Remove that possibility
            if(this.value != ""){
                this.lastValue = this.value;
            }
            // Regex replace nondigits
            this.value = this.lastValue.replace(/[^0-9]/g, '');
            // Update last valid field
            this.lastValue = this.value;
        };
        // Clears value if its invalid when the element loses focus 
        this.onblur = ()=>{
            if(this.value == "" || isNaN(Number(this.value))){
                this.value = 0;
            }
        }
    }
}

customElements.define("number-picker",NumberPicker,{
    extends: "input"
});