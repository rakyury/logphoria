### Node.js Logphoria Package documentation

Instantiate the Logphoria client as follows:

```javascript
var Logphoria = require('logphoria').createClient('my_api_key');
```

Then use the log function as follows:

```javascript
Logphoria.log({type: 'INFO', 
		      message: 'Message from node.js',
		      metadata: { prop1: "1", prop2: 2 }},
		      function(err, result) {
		          	if (err) console.log(err);
			  		console.log(result);
			  });
```

More docs coming soon.