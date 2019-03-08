# CoAP resource usage profiling and anomaly detection


## TODO:
* get some sample data from: https://opensourceforu.com/2016/09/coap-get-started-with-iot-protocols/
* Compute information about the whole dataset - num. of packets, num. of flows, num of profiles, etc.
* Compute "stability" - compare distributions for different number of samples
* Add other methods not based on probability, e.g., SVM, decision trees, ...
* Evaluate on other datasets

## CoAP communication patterns
The simplest communication patterns is request and immediate response:
```
Client              Server
  |                  |
  |   CON [0x7a10]   |
  |    GET /temp     |
  +----------------->|
  |                  |
  |   ACK [0x7a10]   |
  |  2.05 Content    |
  |<-----------------+
```
More complicated pattern is when the response is delayed:
```
Client              Server
  |                  |
  |   CON [0x7a10]   |
  |    GET /temp     |
  |   (Token 0x23)   |
  +----------------->|
  |                  |
  |   ACK [0x7a10]   |
  |<-----------------+
  |                  |
  ... Time Passes  ...
  |                  |
  |   CON [0x23bb]   |
  |  2.05 Content    |
  |   (Token 0x23)   |
  |<-----------------+
  |                  |
  |   ACK [0x23bb]   |
  +----------------->|
```
## UDP Flows
CoAP uses UDP data transfer. UDP communication channel can be used for sending multiple CoAP messages. 



## Method

The method is based on monitoring Coap Resources.
In general, CoAP resources can have one of the following characteristics:

* NORMAL - normal resources are static resources, usually accessed by GET, PUT or POST 
* PERIODIC - represents resources that are regular events. Often these resources are observed. The observation pattern is also regular.                                                                              
* EVENT - represents an irregular events. The resources can be observed.

Method monitors CoAP communication and learns patterns to create a system-wide profile. 

The profile can be computed at the different level of granularity:
## Ip Flow Level

```
Key: ip.src ip.dst coap.code coap.type
Group by: coap.uri_path
Value: sum(udp.length)
```
## Udp Flow Level

## Coap Flow Level

## References
* CoAP Implementation Guidance https://tools.ietf.org/id/draft-ietf-lwig-coap-05.html#rfc.section.2.3
