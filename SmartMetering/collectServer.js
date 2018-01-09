var lwm2m = require('lwm2m-node-lib'),
    timers = require('timers'),
   async = require('async');

   var config = {};
config.server = {
    port: 56840,                         // Port where the server will be listening
    lifetimeCheckInterval: 1000,        // Minimum interval between lifetime checks in ms
    udpWindow: 100,
    defaultType: 'Device',
    logLevel: 'FATAL',
    ipProtocol: 'udp4',
    serverProtocol: 'udp4',
    formats: [
        {
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
    ],
    writeFormat: 'application-vnd-oma-lwm2m/text'
};

function handleError(error) {
    console.log('\nError: %s\n', error);    
}

function handleResult(message) {
    return function(error) {
        if (error) {
            handleError(error);
        } else {
            console.log('\nSuccess: %s\n', message);
        }
    };
}

/**
 * Reads values from the specified smart meter and prints them out. 
 * 
 * @param {String} endpoint     The name of the endpoint to read valuse from.
 */ 
function readValuesFromSmartMeter(endpoint)
{
    console.log('\nReading values from device \n');    
}
  
function registrationHandler(endpoint, lifetime, version, binding, payload, callback) {
    console.log('\nDevice registration:\n----------------------------\n');
    console.log('Endpoint name: %s\nLifetime: %s\nBinding: %s', endpoint, lifetime, binding);

    if (endpoint == 'smart-meter') {
        timers.setInterval(readValuesFromSmartMeter, 5000, endpoint);
    }

    callback();
}

function unregistrationHandler(device, callback) {
    console.log('\nDevice unregistration:\n----------------------------\n');
    console.log('Device location: %s', device);
    callback();
}

function setHandlers(serverInfo, callback) {
    globalServerInfo = serverInfo;
    lwm2m.server.setHandler(serverInfo, 'registration', registrationHandler);
    lwm2m.server.setHandler(serverInfo, 'unregistration', unregistrationHandler);
    callback();
}

function start() {
    async.waterfall([
        async.apply(lwm2m.server.start, config.server),
        setHandlers
    ], handleResult('Lightweight M2M Server started'));
}

function stop() {
    if (globalServerInfo) {
        lwm2mServer.stop(globalServerInfo, handleResult('COAP Server stopped.'));
    } else {
        console.log('\nNo server was listening\n');
    }
}

// executue server
start();

// start monitoring modules

