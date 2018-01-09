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
    moment = require('moment'),
    schedule = require('node-schedule');
    

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
var dataReferenceTime = new Date(argv.origin);
var realReferenceTime = new Date(Date.now());

var rangeRx = /\[(\d+)-(\d+)\]/;
var x = rangeRx.exec(argv.range);
if (x.length != 3) {
    console.log('Error: Invalid range specification. It should have form of [FROM-TO].');
    process.exit(1);
}

var indexFrom =  Number(x[1]);
var indexTo =  Number(x[2]);
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
var currentPowerDemand = [];  

/**
 * Called to update the current meassurement of the specific meter.
 */
function updateValues() {
    var voltage = 230 + randomIntInc(-1,1);
    var currentSum = 0;
    var demandSum = 0;
    currentPowerDemand.forEach(function(element, index) {
        function updateObjectValues(object)
        {
            var referenceDemand = Number(element);
            var demand = referenceDemand + randomIntInc(-referenceDemand/10,referenceDemand/10);
            var consumption = Number(object.attributes[4] != null ? object.attributes[4] : 0) + demand / (60*60);
            var current = demand/voltage;
            currentSum += current;
            demandSum += demand;
            object.attributes[1] = voltage.toString();
            object.attributes[2] = current.toString();
            object.attributes[3] = demand.toString();
            object.attributes[4] = consumption.toString();

            console.log('\n%s: Meter %s:\tVoltage: %s V\tCurrent: %s A\tDemand: %s W\tConsumption: %s kWh', new Date(), index+1, voltage, current, demand, consumption / 1000);            
        }
        client.registry.get('/7001/' + index.toString(), function (error, value) { 
            if (error) {
                client.registry.create('/7001/'+ index.toString(), function (error, value) { if (!error) updateObjectValues(value); });
            } else {
                updateObjectValues(value);   
            }});
            
     });

    client.registry.get('/7000/0' , function (error, object) { 
        if (!error) {
            object.attributes[5] =  voltage.toString();
            object.attributes[6] =  currentSum.toString();
            object.attributes[7] =  demandSum.toString();
            object.attributes[8] =  Number(object.attributes[8] != null ? object.attributes[8] : 0) + demandSum / (60*60);
            console.log('\n%s: Location:\tVoltage: %s V\tCurrent: %s A\tDemand: %s W\tConsumption: %s kWh', new Date(), object.attributes[5], object.attributes[6], object.attributes[7] , Number(object.attributes[8]) / 1000);            
        } 
    });
}

setInterval(updateValues, 1000);

/**
 * 
 * @param {Date} time 
 * @param {Array} values 
 */
function setReferenceValues(time, values) {
    var futureTime = new Date((time.valueOf() - dataReferenceTime.valueOf()) + realReferenceTime.valueOf() + 2000);
    if ( isNaN( futureTime.valueOf() ) ) {  
        console.log('Invalid time!');    
    } else {
        //console.log('Scheduled value update: %s.', futureTime);
        schedule.scheduleJob(futureTime, function() {
            currentPowerDemand = values;
        });
    }
}

csv.fromStream(stream, {headers : true, objectMode : true, delimiter : ';'})
   .on("data", function(data){
         var time = moment(data.time, 'DD/MM/YYYY HH:mm', true);
         var vals = Object.values(data).slice(indexFrom, indexTo+1);
         setReferenceValues(time, vals);
    })
   .on("end", function(){
         console.log("All source data loaded.");
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
