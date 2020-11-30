 **Payment Gateway**
===
		
A simple API that simulates transaction payments.


Getting Started
---
Prerequisites: 
This project requires .Net 5.0 installed. 

	* Clone the project 
	* Open PaymentGateway.sln
	* Build both projects
	* Run PaymentGatewayTests to test the API
	* Run PaymentGateway to run the API

	
Alternativelly you can use Docker to run the API:

	* 	Navigate to /PaymentGateway folder
	*  run docker build -t dockerwebapi -f Dockerfile .     to build the docker image
	*  run docker run -ti --rm -p 8080:80 dockerwebapi      to run the container
	*  Navigate to localhost:8080/swagger/index.html <--I left swagger even in production mode.	