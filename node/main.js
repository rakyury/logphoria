var https = require('https'); 
var http = require('http'); 

function LogphoriaClient(api_key) {
    this.apiKey = api_key;
    
    this.log = function(type, msg, meta, callback) { 
		this.postData({ type: type, message: msg, metadata: meta }, callback);
    }
        
    this.postData = function(obj, callback) {

		var json = JSON.stringify(obj);
		var post_options = {
	    		host: '176.34.114.130',
	    		path: '/log/' + this.apiKey,
	    		method: 'POST',
	    		headers: {
					'Content-Type': 'application/json',
					'Content-Length': json.length
	    		}
	    }
		
	
		var post_req = https.request(post_options, function(res) {
			res.setEncoding('utf8');
			res.on('data', function (chunk) {
				if (callback) {
			    	return callback(null, chunk);
				}
			});

		});
	
		post_req.on('error', function(e) {
			if (callback) {
		    	return callback(e);
			}
		});
	
		post_req.write(json);
		post_req.end();
    }
}

exports.createClient = function(api_key) {
    var logphoria;
    logphoria = new LogphoriaClient(api_key);
    
    return logphoria;
}

