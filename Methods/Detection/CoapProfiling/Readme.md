# CoAP resource usage profiling and anomaly detection
Ironstone.Analyzers.CoapProfiling is a dotnet core console application that implements a method for classification of CoAP network flows. 
The classification is based on the identification of communication patterns using a statistical model.  
Currently, the application requires a csv input file containing decoded PCAP packets. This input file can be created using `tshark` as follows:

```
tshark -T fields -e frame.time_epoch -e ip.src -e ip.dst -e udp.srcport -e udp.dstport -e udp.length -e coap.code -e coap.type -e coap.mid -e coap.token -e coap.opt.uri_path_recon -E header=y -E separator=, -Y "coap && !icmp" -r <INPUT>  > <OUTPUT-CSV-FILE>
```

## Usage

The application provides learning and classification modes. Learning  mode serves for creating a profile from 
provided CoAP communication. Classification mode applies previously created profile to identify known communication.

### Learn the profile

To create a profile the application is executed with `Learn-Profile` command. Two mandatory arguments are expected.
The first argument is a CSV file representing CoAP communication, which is used to compute statistical models for CoAP resources. 
The tool will write the computed profile to the output file specified as the second argument.  

```
dotnet Ironstone.Analyzers.CoapProfiling.dll Learn-Profile -InputFile=SampleData\coap-learn.csv -WriteTo=SampleData\coap.profile
```

### Print the profile
The profile is stored in a binary file. To print all models contained in the profile use `Print-Profile` command:

```
dotnet Ironstone.Analyzers.CoapProfiling.dll Print-Profile -InputFile=SampleData\coap.profile
```

The output has a form of the table that gives us an overview of created models. The content of the table for sample data is as follows:

| Name                          | Observations | Threshold           | Distributions(Packets,Octets) | Mean(Packets,Octets)              | Variance(Packets,Octets)                |
|-------------------------------|--------------|---------------------|-------------------------------|-----------------------------------|-----------------------------------------|
| 2.05(Content)[CON]:           | 10640        | 0.163445422090833   | Fn(x; S),Fn(x; S)             | 1.0015037593985,12.018045112782   | 0.00150163923767492,0.21623605022515    |
| 0.00(Empty)[ACK]:             | 10639        | 0.163441696336995   | Fn(x; S),Fn(x; S)             | 1.00150390074255,12.0180468089106 | 0.00150178018312797,0.216256346370518   |
| 0.03(Put)[CON]:/floor_1_light | 957          | 0.00102299796251294 | Fn(x; S),Fn(x; S)             | 1,37.4994775339603                | 0,0.250261233019855                     |
| 2.04(Changed)[ACK]:           | 1891         | 0.153358540710201   | Fn(x; S),Fn(x; S)             | 1.00052882072977,12.0063458487573 | 0.000528820729772643,0.0761501850872505 |
| 0.03(Put)[CON]:/floor_1_temp  | 935          | 0.177554014211856   | Fn(x; S),Fn(x; S)             | 1.00106951871658,29.0310160427807 | 0.00106951871657757,0.899465240641688   |

The first column lists names of all models. A model is computed for every observed CoAP resource operation. The second column tells us how many flows were used to compute each model. The third column contains threshold values used to evaluate computed scores. The rest columns provide various information on models.

### Classify the communication 

```
dotnet Ironstone.Analyzers.CoapProfiling.dll Test-Capture -ProfileFile=SampleData\coap.profile -InputFile=SampleData\coap-test.csv
```

## The Method

In this section, principles of the implemented method are explained.

### CoAP communication patterns

The simplest CoAP communication pattern is a request followed by the immediate response:

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

More complicated pattern is when the data response is delayed. In this case the server response with ACK 
message. The message containing data is send later with a new Message Id. The `token` value is used to identify all 
messages of this transaction.

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

### UDP Flows

CoAP uses UDP data transfer. UDP communication channel can be used for sending multiple CoAP messages. 



### Statistical Model

The method is based on monitoring operations manipulating with CoAP Resources. In general, CoAP resources can have one of the following characteristics:

* NORMAL - normal resources are static resources, usually accessed by GET, PUT or POST. 
* PERIODIC - represents resources that are regular events. Often these resources are observed. The observation pattern is also regular.                                                                              
* EVENT - represents an irregular events. The resources can be observed but the observation event are aperiodic.

The proposed method monitors CoAP communication and learns patterns of resource usages to create a system-wide profile. 
The pattern has form of a statistical model. The model relates a resource operation to a number of messages and a total number of bytes.
The profile is always computed for the specific window size; default is 60s. The profile can be computed at the different level of granularity.
For example, the group key represented by `(ip.src ip.dst)` pair is used for L3 level flows.
Other levels are given in the table bellow. 
Each flow is used as an input for fitting the corresponding model. For each  `(coap.code, coap.type, coap.uri_path)` tuple a distinct model is created. The model uses 
`cflow.packets` and `cflow.octets` as samples to fit the probabilistic distribution. Thus each model
is represented by two Probabilistic Density Functions. 


| Flow     |  Key |
| -------- | --------------------------------------------------------------- |
| L3 Ip    | (ip.src, ip.dst)                                                 |
| L4 Udp   | (ip.src, udp.srcport, ip.dst, dst.dstport)                       |
| L3 Coap  | (ip.src, udp.srcport, ip.dst, dst.dstport, coap.mid, coap.token) |


## Evaluation 
TODO: Describe available datasets and their parameters.


## TODO:
* get some sample data from: https://opensourceforu.com/2016/09/coap-get-started-with-iot-protocols/
* Compute information about the whole dataset - num. of packets, num. of flows, num of profiles, etc.
* Compute "stability" - compare distributions for different number of samples
* Add other methods not based on probability, e.g., SVM, decision trees, ...
* Evaluate on other datasets


## References
* CoAP Implementation Guidance https://tools.ietf.org/id/draft-ietf-lwig-coap-05.html#rfc.section.2.3
