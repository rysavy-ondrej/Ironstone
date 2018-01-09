#!/usr/bin/env node
/*
 * Smart Meter Client is a demo application that represents a LwM2M client
 * that provides information about enery consumption. 
 * The source data of energy consumption is read from the file provided 
 * when the client starts.
 * 
 * The file has CSV format, each line representing a collection of 
 * actual electricity demand values for different households. 
 * The client is also specified the 1-based indicies of households for which  
 * it provides measured values. 
 * 
 * Values are meassured every second. 
 */
var fs = require('fs'),
    csv = require('fast-csv'),
    argv = require('yargs').argv,
    setInterval = require('timers').setInterval,
    client = require('lwm2m-node-lib').client,  
    moment = require('moment');


if (argv.file == null || argv.origin == null || argv.range == null)
{
    console.log('Error: Some of the mandatory arguments was missing.');
    console.log('Usage: node smartMeterClient --file data.csv --range [1-10] --origin 2017/01/01');
    process.exit(1);
}
/*
 * An array of column indixes to read household data from.
 */
var households = argv.range;
/*
 * The name of data source CSV file.
 */
var sourceFile = argv.file;
/*
 * Date and time used to shift the source data. 
 */
var dataReferenceTime = new moment(argv.origin);
var realReferenceTime = new moment(Date.now);

var indexFrom = argv.range ??;
var indexTo = argv.range ??;

/*
 * Host or IP address of the LwM2M registration server.
 */ 
var serverAddress = argv.server != null ? argv.server : 'localhost';
/*
 * Port number at which the registration server listens.
 */ 
var serverPort = argv.port != null ? argv.port : '5684';

console.log('Reading data from %s', sourceFile);
var stream = fs.createReadStream(sourceFile);

function randomIntInc (low, high) {
    return Math.random() * (high - low + 1) + low;
}

function handleError(error) {
    console.log('\nError: %s\n', error);    
}

var config = {};

config.client = {
    lifetime: '85671',
    version: '1.0',
    logLevel: 'DEBUG',
    observe: {
        period: 3000
    },
    ipProtocol: 'udp4',
    serverProtocol: 'udp4',
    formats: [
        {
            name: 'lightweightm2m/text',
            value: 1541
        }
    ],
    writeFormat: 'lightweightm2m/text'
};

client.init(config, function () {
    console.log('Inititated.');    
});

var serialNumber = argv.serial ? argv.serial : Math.floor(randomIntInc(100000, 999999));

// create a single mandatory object - DEVICE(3)
client.registry.create('/3/0', function (error, value) { if (error) handleError(error); });
// manufacturer
client.registry.setResource('/3/0', '0', 'Ironstone', function (error, value) { if (error) handleError(error); } );
// model number
client.registry.setResource('/3/0', '1', '1.0', function (error, value) { if (error) handleError(error); } );
// serial number
client.registry.setResource('/3/0', '2', serialNumber.toString(), function (error, value) { if (error) handleError(error); } );
// create a smart meter object - SmartMeter(7000)
client.registry.create('/7000/0', function (error, value) { if (error) handleError(error); });

/**
 * Called to update the current meassurement of the specific meter.
 */
function updateValues() {

    // we randomize voltage and compute the rest values:
    var volts = 230 + randomIntInc(-1,1);
    var amperes = randomIntInc(0,2);
    var watts = volts * amperes / 1000;
    // Voltage
    client.registry.setResource('/7000/0', '0', volts.toString(), function (error, value) { if (error) handleError(error); } );
    // Ampere
    client.registry.setResource('/7000/0', '1', amperes.toString(), function (error, value) { if (error) handleError(error); } );
    // Watt
    client.registry.setResource('/7000/0', '2', watts.toString(), function (error, value) { if (error) handleError(error); } );
    
    console.log('%s: Measurement: %sV, %s mA, %s kWh.', new Date(), volts, amperes, watts);
}

var currentPowerDemand = [];

function setReferenceValues(time, values) {
    var futureTime = (time - dataReferenceTime) + realReferenceTime;
    console.log('Scheduled value update: %s.', futureTime);
    futureTime.timer({loop:false}, function()
    {
        currentPowerDemand = values;
    });
}

csv.fromStream(stream, {headers : true, objectMode : true, delimiter : ';'})
   .on("data", function(data){
         var time = new moment(data.time);
         var now = moment.now;
         var vals = Object.values(data).slice(indexFrom, indexTo);
         console.log(vals);
         setReferenceValues(time, vals);
    })
   .on("end", function(){
         console.log("done");
    });

console.log('Registering: Server address: %s:%s.', serverAddress, serverPort);
client.register(serverAddress, serverPort, '', 'smart-meter', function (error, deviceInfo) {
    if (error) {
        handleError(error);
    } else {
        console.log('\n-> Connected: Device location: %s.', deviceInfo.location);
    }
});

client.registry.list(function(error, objList) {
    if (error){
        clUtils.handleError(error);
    } else {
        console.log('\n-> Local objects:');
        for (var i=0; i < objList.length; i++) {
            console.log('\t-> ObjURI: %s / Obj Type: %s / Obj ID: %s / Resource Num: %d',
                objList[i].objectUri, objList[i].objectType, objList[i].objectId,
                Object.keys(objList[i].attributes).length);
        }
    }
});
