## What is this?
This is a simple Proxy server which fully bypasses [CORS](https://developer.mozilla.org/en-US/docs/Web/HTTP/CORS) and allow the browser to query any API.


## Why did I make this?
I found many CORS proxies, however, they simply added “Access-Control-Allow-Origin” to all requests. They did not respond to the CORS preflight requests. Without this, the API calls would fail.


## How does it work?

 - Responds to [CORS preflight requests](https://developer.mozilla.org/en-US/docs/Glossary/Preflight_request)
	 1. Default header values (configurable)
		 - Access-Control-Allow-Origin = *
		 - Access-Control-Allow-Method = *
		- Access-Control-Allow-Headers = *
		- Access-Control-Max-Age = 86400
 2. Adds “Access-Control-Allow-Origin” to all responses
