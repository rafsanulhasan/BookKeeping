function setItem(key, value) {
	var jsonValue = JSON.stringify(value);
	console.log(`${key}: ${jsonValue}`);
	localStorage.setItem(key, jsonValue);
}

function getItem(key) {
	console.log(`get ${key}`);
	return JSON.parse(localStorage.getItem(key));
}
function getItems(key) {
	console.log(`gets ${key}`);
	return JSON.parse(localStorage.getItem(key));
}