var lwm2m = require('lwm2m-node-lib'),
    timers = require('timers'),
    async = require('async');

var config = {};
config.server = {
    port: 5684,                     // Port where the server will be listening
    lifetimeCheckInterval: 1000,    // Minimum interval between lifetime checks in ms
    udpWindow: 100,
    defaultType: 'Device',
    logLevel: 'FATAL',
    ipProtocol: 'udp4',
    serverProtocol: 'udp4',
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
    ],
    writeFormat: 'application-vnd-oma-lwm2m/text'
};

function precisionRound(number, precision) {
    var factor = Math.pow(10, precision);
    return Math.round(number * factor) / factor;
  }

function handleError(error) {
    console.log('\nError: %s\n', error);
}

function handleResult(message) {
    return function (error) {
        if (error) {
            handleError(error);
        } else {
            console.log('\nSuccess: %s\n', message);
        }
    };
}

function registrationHandler(endpoint, lifetime, version, binding, payload, callback) {
    console.log('\nDevice registration:\n----------------------------\n');
    console.log('Endpoint name: %s\nLifetime: %s\nBinding: %s', endpoint, lifetime, binding);
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
        lwm2m.server.stop(globalServerInfo, handleResult('COAP Server stopped.'));
    } else {
        console.log('\nNo server was listening\n');
    }
}


function updateDeviceList()
{
    function readValue(device, resource) {
        lwm2m.server.read(device.id, "7000","0", resource.id, function (error, value) {
            if (error) {
                handleError(error);
            } else {
                console.log(' [%s] -> %s: %s %s', device.id, resource.name, precisionRound(Number(value), 3), resource.units);
            }    
        });
    }

    // start monitoring modules
    var devices = lwm2m.server.listDevices(function (error, deviceList) {
        if (error) {
            //handleError(error);
        } else {
            console.log(new Date().toDateString());
            deviceList.forEach(function(element) {
                readValue(element, { id : "5", name : "voltage", units : "V" } );                                
                readValue(element, { id : "6", name : "current", units : "A" } );                                
                readValue(element, { id : "7", name : "demand", units : "W" } );                                
                readValue(element, { id : "8", name : "consumption", units : "Wh" } ); 
            });
        }
    });                
}

start();
setInterval(updateDeviceList, 10000);