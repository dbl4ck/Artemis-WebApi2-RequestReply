# Artemis-WebApi2-RequestReply
A proof of concept Apache ActiveMQ Artemis + ASP.NET WebAPI2 to model a 
[Brokered Request-Reply Enterprise Architecture Pattern](https://www.enterpriseintegrationpatterns.com/patterns/messaging/RequestReply.html)

# Request-Reply Architecture Pattern

![brokered request-reply enterprise pattern](https://github.com/dbl4ck/Artemis-WebApi2-RequestReply/blob/master/Docs/Media/request-reply.png)

# This Is It In Action

![animation of get/post orders](https://github.com/dbl4ck/Artemis-WebApi2-RequestReply/blob/master/Docs/Media/post-and-get-ani.gif.gif)


# Sub-Project Overview

* **AMQRR.Caller** Console Runnable - POSTS randomly generated *Order* objects at *.API* with a HttpClient and waits for a response.
* **AMQRR.API** ASP.NET WebAPI2 Runnable -	Receives *Order* objects and sends them to a message queue with a request-based *correlationId* and waits for a response message with the same *correlationId* from a *reply queue*.
* **AMQRR.Common** Shared Class Library - Contains shared models and reusable common classes.
* **AMQRR.Processor** Console Runnable - Consumes messages from a messagequeue, before returning a new message with the same *correlationId* into a *reply queue*.

# Class Highlights

## AMQRR.Caller

The [main process](https://github.com/dbl4ck/Artemis-WebApi2-RequestReply/blob/master/AMQRR.Caller/Program.cs) generates a random *CustomerId* and then uses [RandomOrderFactory](https://github.com/dbl4ck/Artemis-WebApi2-RequestReply/blob/master/AMQRR.Common/Factories/RandomOrderFactory.cs) to create randomised [Order](https://github.com/dbl4ck/Artemis-WebApi2-RequestReply/blob/master/AMQRR.Common/Models/Order.cs) and child [OrderLine](https://github.com/dbl4ck/Artemis-WebApi2-RequestReply/blob/master/AMQRR.Common/Models/OrderLine.cs) instances with per-caller incremental *OrderId* & *OrderLineId* counters.

These Order instances are then submitted to the *api/orders/post* http action on the **AMQRR.API**, one every second.

A response is expected from each request, containing the same Order & child OrderLines. 

20 seconds is given by the HttpClient before failure, roundtrip time is displayed.

## AMQRR.API

[MqController](https://github.com/dbl4ck/Artemis-WebApi2-RequestReply/blob/master/AMQRR.API/Base/MqController.cs) Superclass to handle the grunt of the work for both *Request-Reply* and *Point-To-Point* brokering interactions with Apache Artemis, running on a default localhost instance ([port 61616](https://github.com/dbl4ck/Artemis-WebApi2-RequestReply/blob/master/AMQRR.Common/Configuration/Url.cs)).

This is in turn inherited by [OrdersController](https://github.com/dbl4ck/Artemis-WebApi2-RequestReply/blob/master/AMQRR.API/Controllers/OrdersController.cs) which only has to worry about queue selection and serialization concerns.

*Request-Response* demands a *reply queue* and a *timeout/expiry*, as the external caller will likely be waiting for a response within a standard http timeout (20 secs), waiting any longer would serve no purpose. Broker messages are additionally supplied with an *expiry* to match this, a separate *reply queue*, and a *correlationID* so that the request can quickly identify the correct reply message to consume when available.

*Point-To-Point* carries no enforced expiry or reply queue concerns, by definition it only needs to ensure that the message reaches the broker.

[MqService](https://github.com/dbl4ck/Artemis-WebApi2-RequestReply/blob/master/AMQRR.API/Services/Singleton/MqService.cs) singleton is spawned during [Global.asax](https://github.com/dbl4ck/Artemis-WebApi2-RequestReply/blob/master/AMQRR.API/Global.asax.cs) so that broker *Connection* and *Session* instance can be re-used across controller/action invokes.

## AMQRR.Processor

The [main process](https://github.com/dbl4ck/Artemis-WebApi2-RequestReply/blob/master/AMQRR.Processor/Program.cs) opens two listener delegates on two queues.

The [handlers](https://github.com/dbl4ck/Artemis-WebApi2-RequestReply/blob/master/AMQRR.Processor/Handlers/Orders.cs) 

## AMQRR.Common

Contains shared models and libraries primarily for the API and Processor.

[MqSession](https://github.com/dbl4ck/Artemis-WebApi2-RequestReply/blob/master/AMQRR.Common/MQ/MqSession.cs) is used to collate the instantiation of a broker client and is used in both of these projects.

# To Infinity

### 1 Caller to 3 Processors

![1 Caller to 3 Processors](https://raw.githubusercontent.com/dbl4ck/Artemis-WebApi2-RequestReply/master/Docs/Media/Media1-to-3.png)

### 14 Callers to 1 Processors

![14 Callers to 1 Processor](https://raw.githubusercontent.com/dbl4ck/Artemis-WebApi2-RequestReply/master/Docs/Media/Media14-to-1.png)

### 14 Callers to 3 Processors

![14 Callers to 3 Processors](https://raw.githubusercontent.com/dbl4ck/Artemis-WebApi2-RequestReply/master/Docs/Media/Media14-to-3.png)
