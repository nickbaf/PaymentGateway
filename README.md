 **Payment Gateway**
===
		
This document describes the project in general, how to run it, the design and assumptions I've made while developing this API.


Getting Started
---
Prerequisites: 
> This project requires .Net 5.0 installed. 

	* Clone the project 				git clone https://github.com/nickbaf/PaymentGateway.git
	* Go to /PaymentGateway/PaymentGateway          cd PaymentGateway   
	* Build projects                                dotnet publish
	* Run PaymentGatewayTests to test the API       
	* Run PaymentGateway to run the API


	
Alternativelly you can use Docker to run the API:

	*  Navigate to /PaymentGateway/PaymentGateway folder
	*  run docker build -t paymentgateway -f Dockerfile .     to build the docker image
	*  run docker run -ti --rm -p 8080:80 paymentgateway      to run the container
	*  Navigate to localhost:8080/swagger/index.html          I left swagger in production mode as well.



API Design
===

I designed this API with extensibility in mind, opted for as loosely coupled architecture as posible.
In my initial design I opted to use the Command and Query Responsibility Segregation(CQRS) pattern but this would cause an over-egnineered product as our API did not interfere with any other entity. But, I wanted to separate the consern of receiving a request and making checks(that is credit card checks, capture money checks) and operations such as detracting money etc. For this reason I build my API on 2 piles:

Commands
---
A command is a sequence of actions that are required for a requested operation to succeed.

Example:
An Authorization command has the following sequence:

* Validating the credit card
* Validating the amount to be captured

A Capture command has the following sequence:

* Validating the amount to be captured
* Validating that the transaction result will be valid, i.e not end up with negative amount.

A Refund command has the following sequence:

* Validating the amount to be refunded
* Validating that the transaction result will be valid, i.e not end up refunding more than we captured.

Each command is executed and uppon completion it produces an Event and an error list.

Events
---
An Event is raised after the completion of the execution of a command. Each command has 2 accompanying events that can be raised and they can be used for further proccess of a transaction.

**SuccessEvents**
These events are raised when a command is executed succesfully. They return no errors.

**FailiureEvents**
These events are raised when a command is failed to be executed because of an error. They return a list of errors to be attached to the API response.

**The reasons to choose this arcitecture include:**

* Separation of conserns, as making commands and events in this API, those models are way more maintainable and flexible for future updates and changes. In a more complex business logic(e.x one more step in credit card validation) the command or event models can change with ease.
* Security, we can ensure that commands are executed by specific entities of our system.
* 
**Posible drawbacks include(for this solution)**

* Complexity,in the overall solution and some over-engineered components.

Transaction
---
A transaction represents the result of a *Sucessfull Authorization* .

A transaction includes the customer's credit card(CVV removed for security reasons) the amount to be captured, the amount already captured and a transaction strategy i.e capture or refund.

Other components
===

Card
---

Represents a credit card, with number and expiration date. At the beginning I opted to store the CVV in a SecureString. Although I've used them before and I know how to handle them with unsafe code, ptrs etc. I think that security wise, its not good enough. So I opted for a plain string.

Money
---
Represents an amount of money with its respective currency.

TransactionBucket
---
Represents the in memory object that stores all transactions.

**Logging**
---
Logging is implemented by using the NLog library.

Testing
===
For testing purposes I used the NUnit library allongside with NFluent for easier assertions and Moq for mocking my dependencies. For all testing cases I used credit card numbers found on the web(mock ones NOT stollen ofc).


**Assumptions**
==
Mock Bank:
---
In last day of development I implemented a Mock Bank interface that all it does is create some latency into the API in order to simulate a mock communication with the bank. TBH, it was a rushed dicision, and it might not add anything to the project.My general assumption for this API was that we are dealing with all checks regarding card, money, card balance etc. and that there is a Bank at the other side of the line waiting to hear our command which will be executed sucessfully.


When a transaction ends?
---

The answer is never,after we capture the whole amount in a transaction the object is stored in the dictionary as long as the API lives. I thought about implementing a "End" endpoing that will mark the transaction complete, but that is for version 2.

**LuhnCheck**

I implemented the Luhn's algorithm for validating a credit card from an pseudo-code I found on....Wikipedia. I tested the algorithm using a bunch of credit cards I found on the web(!) as well as with my own cards, and it seems to work OK. 


**Self evaluation**
==
Looking at the solution now, Monday November 30th; after 5 days of development I feel like some aspects could have been done in a better and more elegant way.

 These include:

* The TransactionBucket which represents the in memory object that all the transactions are stored should have been in a higher level in the system and part of the Events class that in the CQRS design writes the changes.
* I should have created a database controller with a database like LiteDB to store the transactions as the in memory object doesn't represent 100% what we have in real life payment systems. Althrough we need information to be stored in memory for efficiency we need long term storage in the db as well.
* CQRS model might have some architecture problems as the separation of Command and Event to my eyes is not 100% complete. I should have implemented a kind of communicaton between those 2 objects so as to run separately.(My first time implementing this, but dont gonna sell it as an excuse.)
* Tests,Tests,Tests. Wrote only 34, should have written more.
* Code and naming conventions...Ahh this is a part where I lack discipline, and I'm sure you will whitness some violations in coding style and naming.
* Mediator pattern should have been used between Commands and Events.

**Lessons learned**
---
* TDD is cool. Starting the project I created 2 solutions, for the API and its respective tests. I started developing the project with TDD and all I have to say is that I probably saved a bunch of hours fixing bugs. 
* Reading Microsoft documentation is not as bad as I thought. Some times, I feel overwhelmed for not following best practices and creating too much techincal debt, and in my quest creating quality code over the past year I deliberatelly avoided Microsoft's documentation regarding its products and relied on thrird-party docs. Turns out, Microsoft's engineer's written documentation about ASP.NET its not bad at all. After all they create these frameworks. ¯\_(ツ)_/¯




	
