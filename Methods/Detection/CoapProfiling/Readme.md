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

The method is based on monitoring operations with Coap Resources. In general, CoAP resources can have one of the following characteristics:

* NORMAL - normal resources are static resources, usually accessed by GET, PUT or POST. 
* PERIODIC - represents resources that are regular events. Often these resources are observed. The observation pattern is also regular.                                                                              
* EVENT - represents an irregular events. The resources can be observed but the observation event are aperiodic.

The proposed method monitors CoAP communication and learns patterns of resource usages to create a system-wide profile. 
Regardless of a type of the resource the pattern is defined by a statistical model considering number of messages and their size 
representing the CoAP operation. 

The profile is always computed for the specific window size, default is 60s. The profile can be computed at the different level of granularity. 
CoAP packets are aggregated into flows. For example, the group key represented by `(ip.src ip.dst)` pair is used for L3 level flows. 
Each flow is used as an input for fitting the corresponding model. For each  
`(coap.code, coap.type, coap.uri_path)` tuple distinct model is created. The model uses 
`cflow.packets` and `cflow.octets` as samples to fit the probabilistic distribution. Thus each model
is represented by two Probabilistic Density Functions. 


| Flow     |  Key |
| -------- | --------------------------------------------------------------- |
| L3 Ip    | (ip.src, ip.dst)                                                 |
| L4 Udp   | (ip.src, udp.srcport, ip.dst, dst.dstport)                       |
| L3 Coap  | (ip.src, udp.srcport, ip.dst, dst.dstport, coap.mid, coap.token) |

## Evaluation 
TODO: Describe available datasets and their parameters.

## References
* CoAP Implementation Guidance https://tools.ietf.org/id/draft-ietf-lwig-coap-05.html#rfc.section.2.3
