#!/usr/bin/env node
/*
 * Smart Meter Server is a demo application that represents a LwM2M server
 * that collects information about energy consumption provided by registered clients. 
 *
 *  The interval between readings of measured values is by default every 30 second, but it can be configured by --interval argument.
 */
/* 
 * Copyright 2017 Ondrej Rysavy <rysavy@fit.vutbr.cz>
 */

var lwm2m = require('lwm2m-node-lib'),
    timers = require('timers'),
    argv = require('yargs').argv,
    util = require('util'),
    async = require('async');
    ServerConfig = require('./config').ServerConfig;
    Misc = require('./misc').Misc;
// create server configuration
var config = {};
config.server = new ServerConfig();
config.server.port = (argv.port != null) ? argv.port : '56480';                   // Port where the server will be listening
config.server.updateInterval = (argv.interval != null) ? argv.interval : 30,   // The number of seconds between read requests for the measured values.

function handleError(error) {
    Misc.logEvent(error);
    process.exit(1);
}

function handleResult(message) {
    return function (error) {
        if (error) {
            handleError(error);
        } else {
            Misc.logEvent(message);
        }
    };
}

function registrationHandler(endpoint, lifetime, version, binding, payload, callback) {
    Misc.logEvent(util.format('[REGISTERED: Endpoint=%s, Lifetime=%s, Binding=%s]', endpoint, lifetime, binding));
    callback();
}

function unregistrationHandler(device, callback) {
    Misc.logEvent(util.format('[UNREGISTERED: Location= %s]', Misc.getTs(), device));
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
    ], handleResult(util.format("[SERVER-STARTED: Port=%s]", config.server.port))); 
}
function stop() {
    if (globalServerInfo) {
        lwm2m.server.stop(globalServerInfo, handleResult(util.format('[SERVER-STOPPED]')));
    } else {
        Misc.logEvent('\nNo server was listening\n');
    }
}

function updateDeviceList()
{
    function readValue(device, resource) {
        lwm2m.server.read(device.id, "7000","0", resource.id, function (error, value) {
            if (error) {
                handleError(error);
            } else {
                console.log(' ├─[%s]─ %s = %s %s', device.id, resource.name, precisionRound(Number(value), 3), resource.units);
            }    
        });
    }

    // start monitoring modules
    var devices = lwm2m.server.listDevices(function (error, deviceList) {
        if (error) {
            handleError(error);
        } else {
            
            if (deviceList.length === 0) {
                Misc.logEvent(util.format('[NO-DEVICE]', Misc.getTs()));
            }
            else {
                Misc.logEvent(util.format('[READING]'));
                deviceList.forEach(function(element) {
                    readValue(element, { id : "5", name : "voltage", units : "V" } );                                
                    readValue(element, { id : "6", name : "current", units : "A" } );                                
                    readValue(element, { id : "7", name : "demand", units : "W" } );                                
                    readValue(element, { id : "8", name : "consumption", units : "Wh" } ); 
                });
            }
        }
    });                
}

// run the server
start();

// schedule events for reading values from the registered clients:
setInterval(updateDeviceList, config.server.updateInterval * 1000);