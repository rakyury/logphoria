var Logphoria = require('./main.js').createClient('eddfab7a-a992-0311-80f8-bbc22b69a8a1');

Logphoria.log('INFO', 'Test message from node.js', { prop1: "1", prop2: 2}, function (error) {
	console.log(error)
	
})