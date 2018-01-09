# Simple demonstration of LwM2M
This demonstration environment consists of a single server equipped with a simple UI to present information 
read from multiple clients. The clients are installed at 20 measurement locations and each smart meter provide 
information about 10 households. 

To simulate a real world situation, the NREL datatset https://data.nrel.gov/submissions/69 was used as the source of data.
The data set contains values of power demands for 200 households meassured every 10 minutes. 

## Collect Server
The server collects values from registered clients and vizualized them in a simple UI.

## Smart meter client
The Smart Meter client represents a device that measures electricity consumption close to the 
### Objects

| Name                  | Id   | Instance | Mandatory | URN                    |
| --------------------- | ---- | -------- | --------- | ---------------------- |
| SmartMeter            | 7000 | Single   | Yes       | urn:oma:lwm2m:ext:7000 |
| ElectricityMeter      | 7001 | Multiple | Yes       | urn:oma:lwm2m:ext:7001 |

### Smart Meter Properties
The smart meter consists of a number of slots each equipped with an electricity meter. 
Smart meter provides aggregated value for all electricity meter devices. 

| ID  | Name             | OP  | Instances | Mandatory | Type    | Units | Description         |
| --- | ---------------- | --- | --------- | --------- | ------- | ----- | ------------------- |
| 1   | Number of points | R   | Single    | Mandatory | Integer | -     | Specifies a number of active measurement points. |
| 2   | Power Status     | R   | Single    | Optional  | String  | -     | The power status of smart power (operational, failed, initializing, updating, unknown).|
| 3   | Network Status   | R   | Single    | Optional  | String  | -     | The network status of smart meter. |
| 4   | Power Switch     | E   | Multiple  | Optional  | Boolean | -     | Turn on/off the smart meter. |
| 5   | Voltage          | R   | Single    | Mandatory | Float   | V     | The voltage as meassured by the smart meter terminal.  |
| 6   | Current          | R   | Single    | Mandatory | Float   | A     | The total current as meassured by the smart meter terminal. |
| 7   | Demand           | R   | Single    | Mandatory | Float   | W     | The total electricity demand as meassured by the smart meter terminal. |
| 8   | Consumption      | R   | Single    | Mandatory | Float   | kWh   | The total consumption of electricity as meassured by the smart meter terminal. |

### Electricity Meter Device
Each meter provides actual values, such as, voltage, current, and 
electricity demand values, and aggregated value representing total consumption of the single location.  
Also each meter is also equipped with power switch that can be controlled.

| ID  | Name             | OP  | Instances | Mandatory | Type    | Units | Description         |
| --- | ---------------- | --- | --------- | --------- | ------- | ----- | ------------------- |
| 1   | Voltage          | R   | Single    | Mandatory | Float   | V     | The voltage as meassured by the meter device.  |
| 2   | Current          | R   | Single    | Mandatory | Float   | A     | The current as meassured by the meter device. |
| 3   | Demand           | R   | Single    | Mandatory | Float   | W     | The electricity demand as meassured by the meter device. |
| 4   | Consumption      | R   | Single    | Mandatory | Float   | kWh   | The total consumption of electricity as meassured by the meter device. |
| 5   | Power Switch     | E   | Single    | Mandatory | Boolean | -     | Turn on/off the meter device and thus electricity for the consumer. |

In addition to metering, the smart meter provides operational information via status resources. 
