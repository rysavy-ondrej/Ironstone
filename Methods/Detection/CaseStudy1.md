# Coap Anomaly Detection

## Problem Description
Smart home networks with Internet of Things (IoT) devices can be a target of cyber attacks against IoT communication since many IoT protocols are implemented without authentication or encryption. Attackers can intercept IoT communication and also manipulate with IoT devices by sending spoofed commands to IoT sensors or controllers. This case study aims to demonstrate the security issues of IoT environment and to show how anomaly behavior can be detected by application of AI methods.

## Input Data

The sample data set represents monitoring and control data transmission between a client and a server in the test-bed network. The CoAP server is implemented on an IoT device (sensor, actuator, etc.) where it measures physical quantity like temperature and humidity or where it controls motion, light, smoke, etc. For our purposes, we will monitor CoAP communication and extract L7 (application layer) data like a code of the command, token, message ID, and URI. This dataset serves as an input for anomaly-based analysis of security incidents. Apart from regular communication datasets, students are also given samples of selected networks attacks, e.g., an unauthorized resource access, denial of service, etc. The dataset used in this case study consists of a collection of capture files as listed in Table bellow. Files idle.cap, regular.cap and observe.cap contain normal communication that will be used for learning the communication profile. File attack.cap contains samples of anomaly communication for testing the anomaly detection method.

| File | Packets | Flows | Resources | Normal |
|---|---|---|---|---|
|[idle.cap](./CoapProfiling/SampleData/idle.pcapng)       | 25 096   | 716     |  5     |   yes |
|regular.cap  | 54 634   | 1 307  | 12   |   yes  |
|observe.cap | 17 480   | 415    |  8     |   yes  |  
|attack.cap   | 38 474    | 870     |  8     |  no  | 

