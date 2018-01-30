"use strict";
/*
 * Smart Meter Client is a demo application that represents a LwM2M client
 * providing information about actual energy consumption.
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
/*
* Copyright 2017 Ondrej Rysavy <rysavy@fit.vutbr.cz>
*/
Object.defineProperty(exports, "__esModule", { value: true });
var yargs = require("yargs");
var fs_1 = require("fs");
var fast_csv = require("fast-csv");
var moment = require("moment");
var schedule = require("node-schedule");
var misc_1 = require("./misc");
var client = require('lwm2m-node-lib').client;
var format = require('util').format;
function handleError(error) {
    console.log('[ERROR: ts=%s, msg=%s]', misc_1.Misc.getTs(), error);
}
/**
 * This class implements a Smart Meter Client object.
 */
var SmartMeterClient = /** @class */ (function () {
    function SmartMeterClient() {
        var _this = this;
        /**
         * Vector of current demand values for the meassured households.
         */
        this.currentPowerDemand = [];
        /**
         * Loads the input file that contains samples of meassured values.
         * @param path Path to the fsource file.
         * @param columns Range of columns to used.
         * @param reftime Reference time.
         */
        this.loadSourceFile = function (path, columns) {
            misc_1.Misc.logEvent(format('[READING: File=%s]', path));
            var stream = fs_1.createReadStream(path);
            var __this = _this;
            fast_csv.fromStream(stream, { headers: true, objectMode: true, delimiter: ';' })
                .on("data", function (data) {
                var time = moment(data.time, 'DD/MM/YYYY HH:mm', true);
                var array = Object.keys(data).map(function (key) { return data[key]; });
                var values = array.slice(columns.from, columns.to + 1);
                __this.setReferenceValues(time, values);
            })
                .on("end", function () {
                misc_1.Misc.logEvent("[LOADED: msg='Source data load completed.']");
            });
        };
        this.setReferenceValues = function (time, values) {
            var __this = _this;
            var futureTime = new Date(time.valueOf() + _this.timeshift + 5000);
            if (isNaN(futureTime.valueOf())) {
                misc_1.Misc.logEvent(format('[ERROR: msg="Invalid time value", value=%s]', time));
            }
            else {
                schedule.scheduleJob(futureTime, function () {
                    misc_1.Misc.logEvent("[UPDATE: msg='Power demand values updated from dataset.']");
                    __this.currentPowerDemand = values.map(Number);
                });
            }
        };
        /**
         * Starts the client.
         */
        this.start = function (args) {
            var __this = _this;
            _this.config =
                {
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
                    writeFormat: 'lightweightm2m/text',
                    endpointName: args.endpoint,
                    serverAddress: args.address,
                    serialNumber: args.serial,
                    serverPort: args.port
                };
            if (args.file == null || args.origin == null || args.range == null) {
                console.log('Error: Some of the mandatory arguments was missing.');
                console.log('Usage: node smartMeterClient --file data.csv --range [1-10] --origin 2017/01/01');
                process.exit(1);
            }
            var households = args.range;
            /*
             * The name of data source CSV file.
             */
            var sourceFile = args.file;
            /*
             * Date and time used to shift the source data.
             */
            var dataReferenceTime = new Date(args.origin);
            var realReferenceTime = new Date(Date.now());
            _this.timeshift = realReferenceTime.valueOf() - dataReferenceTime.valueOf();
            var rangeRx = /\[(\d+)-(\d+)\]/;
            var x = rangeRx.exec(args.range);
            if (x.length != 3) {
                console.log('Error: Invalid range specification. It should have form of [FROM-TO].');
                process.exit(1);
            }
            var indexFrom = Number(x[1]);
            var indexTo = Number(x[2]);
            __this.loadSourceFile(sourceFile, { from: indexFrom, to: indexTo });
            __this.initialize();
            // update value every second
            setInterval(function () { __this.updateValues(); }, 1000);
            // re-register every 10 minutes
            setInterval(function () { __this.updateRegistration(); }, 1000 * 60 * 10);
        };
        /**
         * Initializes M2M client and creates necessary objects and their resources.
         */
        this.initialize = function () {
            client.init({ client: _this.config }, function () {
                misc_1.Misc.logEvent('[INITIALIZED]');
            });
            // create a single mandatory object - DEVICE(3)
            client.registry.create('/3/0', function (error, value) { if (error)
                handleError(error); });
            // manufacturer
            client.registry.setResource('/3/0', '0', 'Ironstone', function (error, value) { if (error)
                handleError(error); });
            // model number
            client.registry.setResource('/3/0', '1', '1.0', function (error, value) { if (error)
                handleError(error); });
            // serial number
            client.registry.setResource('/3/0', '2', _this.config.serialNumber.toString(), function (error, value) { if (error)
                handleError(error); });
            // create a smart meter object - SmartMeter(7000)
            client.registry.create('/7000/0', function (error, value) { if (error)
                handleError(error); });
        };
        this.updateRegistration = function () {
            client.register(_this.config.serverAddress, _this.config.serverPort, _this.config.endpointName, function (error, deviceInfo) {
                if (error) {
                    misc_1.Misc.logEvent(format('[REGISTER-ERROR: Server=%s:%s, Endpoint=%s, Error=%s]', this.config.serverAddress, this.config.serverPort, this.config.endpointName, error));
                    handleError(error);
                }
                else {
                    misc_1.Misc.logEvent(format('[REGISTERED: Server=%s:%s, Endpoint=%s, Location=%s]', this.config.serverAddress, this.config.serverPort, this.config.endpointName, deviceInfo.location));
                }
            });
        };
        /**
         * Called to update the current meassurement of the specific meter.
         */
        this.updateValues = function () {
            var voltage = 230 + misc_1.Misc.randomIntInc(-1, 1);
            var currentSum = 0;
            var demandSum = 0;
            console.log('\n%s:', new Date());
            _this.currentPowerDemand.forEach(function (element, index) {
                function updateObjectValues(object) {
                    var referenceDemand = Number(element);
                    var demand = referenceDemand + misc_1.Misc.randomIntInc(-referenceDemand / 10, referenceDemand / 10);
                    var consumption = Number(object.attributes[4] != null ? object.attributes[4] : 0) + demand / (60 * 60);
                    var current = demand / voltage;
                    currentSum += current;
                    demandSum += demand;
                    object.attributes[1] = voltage.toString();
                    object.attributes[2] = current.toString();
                    object.attributes[3] = demand.toString();
                    object.attributes[4] = consumption.toString();
                    console.log('Meter %s:\tVoltage: %s V\tCurrent: %s A\tDemand: %s W\tConsumption: %s kWh', index + 1, misc_1.Misc.precisionRound(voltage, 3), misc_1.Misc.precisionRound(current, 3), misc_1.Misc.precisionRound(demand, 3), misc_1.Misc.precisionRound(consumption / 1000, 3));
                }
                client.registry.get('/7001/' + index.toString(), function (error, value) {
                    if (error) {
                        client.registry.create('/7001/' + index.toString(), function (error, value) { if (!error)
                            updateObjectValues(value); });
                    }
                    else {
                        updateObjectValues(value);
                    }
                });
            });
            client.registry.get('/7000/0', function (error, object) {
                if (!error) {
                    object.attributes[5] = voltage.toString();
                    object.attributes[6] = currentSum.toString();
                    object.attributes[7] = demandSum.toString();
                    object.attributes[8] = Number(object.attributes[8] != null ? object.attributes[8] : 0) + demandSum / (60 * 60);
                    console.log('Location:\tVoltage: %s V\tCurrent: %s A\tDemand: %s W\tConsumption: %s kWh', misc_1.Misc.precisionRound(voltage, 3), misc_1.Misc.precisionRound(currentSum, 3), misc_1.Misc.precisionRound(demandSum, 3), misc_1.Misc.precisionRound(Number(object.attributes[8]) / 1000, 3));
                }
            });
        };
    }
    SmartMeterClient.main = function () {
        var argv = yargs.command(['$0', 'start'], "Start the smart metering node.", function (yargs) {
            return yargs.option('port', {
                describe: "Port of the server.",
                default: "56480",
            }).option('address', {
                describe: 'IP address of the server.',
                default: 'localhost',
            }).option("endpoint", {
                describe: 'The endpoint name of the smart meter instance.',
                default: 'SmartMeter0000',
            }).option("serial", {
                describe: 'Serial number of this instance.',
                default: 'SM0000X000',
            }).option("range", {
                describe: 'Range of columns in hte source data to load values from.',
                default: '[1-1]',
            }).option("origin", {
                describe: 'The timestamp in YYYY/MM/DD format used as the reference value.',
                default: '',
            }).option("file", {
                describe: 'The path to the file containing source data.',
                required: true
            });
        }, new SmartMeterClient().start);
        argv.parse();
        return 0;
    };
    return SmartMeterClient;
}());
SmartMeterClient.main();
//# sourceMappingURL=smartMeterClient.js.map