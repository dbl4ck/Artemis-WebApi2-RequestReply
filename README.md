# Artemis-WebApi2-RequestReply
A proof of concept Apache ActiveMQ Artemis + ASP.NET WebAPI2 to model a 
[Brokered Request-Reply Enterprise Architecture Pattern](https://www.enterpriseintegrationpatterns.com/patterns/messaging/RequestReply.html)

# Sub-Project Overview

* **AMQRR.Caller** POSTS randomly generated *Order* objects at *.API* with a HttpClient and waits for a response.
* **AMQRR.API**	Receives *Order* objects and sends them to a message queue with a request-based *correlationId* and waits for a response message with the same *correlationId* from a *reply queue*.
* **AMQRR.Common** Contains shared models and reusable common classes.
* **AMQRR.Processor** Consumes messages from a messagequeue, before returning a new message with the same *correlationId* into a *reply queue*.
