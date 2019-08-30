# CoAP resource usage profiling and anomaly detection

*Ironstone.Analyzers.CoapProfiling* is a dotnet core console application that implements a method for classification of CoAP network flows. 
The classification is based on the identification of communication patterns using a statistical model. The application can either read data from pcap file or a CSV input file containing decoded PCAP packets. CSV input file can be created using `tshark` as follows:

```
tshark -T fields -e frame.time_epoch -e ip.src -e ip.dst -e udp.srcport -e udp.dstport -e udp.length -e coap.code -e coap.type -e coap.mid -e coap.token -e coap.opt.uri_path_recon -E header=y -E separator=, -Y "coap && !icmp" -r <INPUT>  > <OUTPUT-CSV-FILE>
```
## Requirements
To run the application, it is required to have installed dotnet core version 2.2. It is available for various systems at:
https://dotnet.microsoft.com/download/dotnet-core/2.2 


## Usage

The application provides learning and classification modes. Learning  mode serves for creating a profile from 
provided CoAP communication. Classification mode applies previously created profile to identify known communication.

### Learning 

To create a profile the application is executed with `Learn-Profile` command. Two mandatory arguments are expected.
The first argument is a CSV file representing CoAP communication, which is used to compute statistical models for CoAP resources. 
The tool will write the computed profile to the output file specified as the second argument.  

```
dotnet Bin\Ironstone.Analyzers.CoapProfiling.dll Learn-Profile -InputCsvFile=SampleData\coap-learn.csv -WriteTo=Profiles\coap.profile
```


### Classification 

Once we have a profile created from the representative samples of CoAP communication, we can use this profile to classify the CoAP flows. The profile is stored in the binary file, which the tool loads and uses to discriminate the input CSV file.  To use the tool in the classification mode, execute it with `Test-Capture` command.

```
dotnet Bin\Ironstone.Analyzers.CoapProfiling.dll Test-Capture -ProfileFile=Profiles\coap.profile -InputCsvFile=SampleData\idle.csv
```
The tool prints the output table for each time window. The table consists of flows each occupying a single row. For each flow, a score is computed. Depending on the calculated score and the corresponding model threshold value the flow is classified. 

```
2/28/2019 8:07:19 PM:
| CoAP Flow                                                               | Packets | Octets | Score                | Threshold           | Anomaly |
|-------------------------------------------------------------------------|---------|--------|----------------------|---------------------|---------|
| 192.168.10.107.47702->192.168.10.104.5683 2.05(Content)[CON]:           | 1       | 12     | 0                    | 0.163445422090833   | False   |
| 192.168.10.104.5683->192.168.10.107.47702 0.00(Empty)[ACK]:             | 1       | 12     | 0                    | 0.163441696336995   | False   |
| 192.168.10.107.51568->192.168.10.105.5683 2.05(Content)[CON]:           | 1       | 12     | 0                    | 0.163445422090833   | False   |
| 192.168.10.105.5683->192.168.10.107.51568 0.00(Empty)[ACK]:             | 1       | 12     | 0                    | 0.163441696336995   | False   |
| 192.168.10.105.36107->192.168.10.107.5683 0.03(Put)[CON]:/floor_1_light | 1       | 37     | 0                    | 0.00102299796251294 | False   |
| 192.168.10.107.5683->192.168.10.105.36107 2.04(Changed)[ACK]:           | 1       | 12     | 0                    | 0.153358540710201   | False   |
| 192.168.10.107.47860->192.168.10.104.5683 2.05(Content)[CON]:           | 1       | 12     | 0                    | 0.163445422090833   | False   |
| 192.168.10.104.5683->192.168.10.107.47860 0.00(Empty)[ACK]:             | 1       | 12     | 0                    | 0.163441696336995   | False   |
| 192.168.10.107.57145->192.168.10.105.5683 2.05(Content)[CON]:           | 1       | 12     | 0                    | 0.163445422090833   | False   |
| 192.168.10.105.5683->192.168.10.107.57145 0.00(Empty)[ACK]:             | 1       | 12     | 0                    | 0.163441696336995   | False   |
| 192.168.10.107.53766->192.168.10.104.5683 2.05(Content)[CON]:           | 1       | 12     | 0                    | 0.163445422090833   | False   |
| 192.168.10.104.5683->192.168.10.107.53766 0.00(Empty)[ACK]:             | 1       | 12     | 0                    | 0.163441696336995   | False   |
| 192.168.10.107.60717->192.168.10.105.5683 2.05(Content)[CON]:           | 1       | 12     | 0                    | 0.163445422090833   | False   |
| 192.168.10.105.5683->192.168.10.107.60717 0.00(Empty)[ACK]:             | 1       | 12     | 0                    | 0.163441696336995   | False   |
```

### Print the profile
The profile is stored in a binary file. To print all models contained in the profile use `Print-Profile` command:

```
dotnet Bin\Ironstone.Analyzers.CoapProfiling.dll Print-Profile -InputFile=Profiles\coap.profile
```

The output has a form of the table that gives us an overview of created models. The content of the table for sample data is as follows:

```
| Name                          | Observations | Threshold           | Distributions(Packets,Octets) | Mean(Packets,Octets)              | Variance(Packets,Octets)                |
|-------------------------------|--------------|---------------------|-------------------------------|-----------------------------------|-----------------------------------------|
| 2.05(Content)[CON]:           | 10640        | 0.163445422090833   | Fn(x; S),Fn(x; S)             | 1.0015037593985,12.018045112782   | 0.00150163923767492,0.21623605022515    |
| 0.00(Empty)[ACK]:             | 10639        | 0.163441696336995   | Fn(x; S),Fn(x; S)             | 1.00150390074255,12.0180468089106 | 0.00150178018312797,0.216256346370518   |
| 0.03(Put)[CON]:/floor_1_light | 957          | 0.00102299796251294 | Fn(x; S),Fn(x; S)             | 1,37.4994775339603                | 0,0.250261233019855                     |
| 2.04(Changed)[ACK]:           | 1891         | 0.153358540710201   | Fn(x; S),Fn(x; S)             | 1.00052882072977,12.0063458487573 | 0.000528820729772643,0.0761501850872505 |
| 0.03(Put)[CON]:/floor_1_temp  | 935          | 0.177554014211856   | Fn(x; S),Fn(x; S)             | 1.00106951871658,29.0310160427807 | 0.00106951871657757,0.899465240641688   |
```

The first column lists names of all models. A model is computed for every observed CoAP resource operation. The second column tells us how many flows were used to compute each model. The third column contains threshold values used to evaluate computed scores. The rest columns provide various information on models.


## References
* CoAP Implementation Guidance https://tools.ietf.org/id/draft-ietf-lwig-coap-05.html#rfc.section.2.3
