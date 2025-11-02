// Collapsable Panel component
// Click to show/hide content
class CollapsePanel extends HTMLElement {
    async connectedCallback(){
        // Grab the html
        const shadow = this.attachShadow({mode:"open"});
        let resp = await fetch("/ui/panel/panel.htm");
        let doc = await resp.text();
        shadow.innerHTML = doc;
        // Grab the CSS
        let css = document.createElement('link');
        css.href = "/ui/panel/panel.css";
        css.rel="stylesheet";
        shadow.appendChild(css);
        
        // Add events
        this.ontoggle = ()=>{}
        
        // Show/Hide Event
        let head = shadow.getElementById("chead");
        let body = shadow.getElementById("cbody");
        let toggle = shadow.getElementById("ctoggle");
        head.onclick = ()=>{
            body.hidden = !body.hidden;
            if(body.hidden){
                toggle.innerText = "+";
            }else [
                toggle.innerText = "-"
            ]
            this.ontoggle(body.hidden);
        }

    }
}
customElements.define("collapse-panel",CollapsePanel);