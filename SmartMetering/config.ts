export class ClientConfig {
    lifetime: string = '85671';
    version: string = '1.0';
    logLevel: string = 'DEBUG';
    observe : {
        period : 3000 
    };
    ipProtocol: string = 'udp4' ;
    serverProtocol :string = 'udp4';
    formats: [
        {
            name : 'lightweightm2m/text',
            value: 1541
        }
    ];
    writeFormat : string = 'lightweightm2m/text';
    endpointName: string;
    serverAddress: string ;
    serverPort:  string; 
    serialNumber:string;
};

export class ServerConfig {
    port: string = '5684';                   // Port where the server will be listening
    updateInterval: number = 30;   // The number of seconds between read requests for the measured values.
    lifetimeCheckInterval: number = 5000;                                   // Minimum interval between lifetime checks in ms
    udpWindow: number = 100;
    defaultType: string = 'Device';
    logLevel: string = 'FATAL';
    ipProtocol: string = 'udp4';
    serverProtocol: string = 'udp4';
    formats: [{
            name: 'application-vnd-oma-lwm2m/text',
            value: 1541
        },
        {
            name: 'application-vnd-oma-lwm2m/tlv',
            value: 1542
        },
        {
            name: 'application-vnd-oma-lwm2m/json',
            value: 1543
        },
        {
            name: 'application-vnd-oma-lwm2m/opaque',
            value: 1544
        }
    ];
    writeFormat: string = 'application-vnd-oma-lwm2m/text';
}