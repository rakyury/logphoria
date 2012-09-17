<?php

# Use of "json_encode()" requires PHP 5.2.0 or greater

class Logphoria {
	
	public $accessKey;
	
	public function __construct($secretKey) {
		$this->accessKey = $secretKey;
		$this->apiUrl = 'http://176.34.114.130/log/'.$this->accessKey;
	}
	
	public function log($type, $msg, array $meta=Null) {
		$parameters = array('type' => $type, 'message' => $msg, 'metaData' => $meta);
		# Convert to JSON
		$jsonData = json_encode($parameters);
		print($jsonData);
		# Make request
		$this->postData($jsonData);
	}

	public function postData($data) {
		### Makes an HTTP POST with JSON data
		$ch = curl_init();
		# SSL options
		#curl_setopt($ch, CURLOPT_SSL_VERIFYPEER, true);
		# Check that common name exists and that it matches server hostname
		#curl_setopt($ch, CURLOPT_SSL_VERIFYHOST, 2);
		#curl_setopt($ch, CURLOPT_CAINFO, getcwd() ."/CAcerts/BuiltinObjectToken-GoDaddyClass2CA.cert");
		# POST options
		curl_setopt($ch, CURLOPT_HTTPHEADER, array('Content-Type: application/json'));
		curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
		curl_setopt($ch, CURLOPT_POST, true);
		curl_setopt($ch, CURLOPT_URL, $this->apiUrl);
		curl_setopt($ch, CURLOPT_POSTFIELDS, $data);
		# Make the request
		$result = curl_exec($ch);
	}
}

?>
