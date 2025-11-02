// Imports other UI components 
async function import_html(url,id){
	//Load URL
	let resp = await fetch(url);
	let reader = resp.body.getReader();
	let {value: bytes } = await reader.read();	
	const decoder = new TextDecoder('utf-8');
	let text = decoder.decode(bytes);
	let elem = document.getElementById(id)
	if (elem == null) {
		console.log('could not find import to id: '+id)
	} else {
		elem.innerHTML = text;
	}
};
