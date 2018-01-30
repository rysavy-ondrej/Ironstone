"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var ClientConfig = /** @class */ (function () {
    function ClientConfig() {
        this.lifetime = '85671';
        this.version = '1.0';
        this.logLevel = 'DEBUG';
        this.ipProtocol = 'udp4';
        this.serverProtocol = 'udp4';
        this.writeFormat = 'lightweightm2m/text';
    }
    return ClientConfig;
}());
exports.ClientConfig = ClientConfig;
;
var ServerConfig = /** @class */ (function () {
    function ServerConfig() {
        this.port = '5684'; // Port where the server will be listening
        this.updateInterval = 30; // The number of seconds between read requests for the measured values.
        this.lifetimeCheckInterval = 5000; // Minimum interval between lifetime checks in ms
        this.udpWindow = 100;
        this.defaultType = 'Device';
        this.logLevel = 'FATAL';
        this.ipProtocol = 'udp4';
        this.serverProtocol = 'udp4';
        this.writeFormat = 'application-vnd-oma-lwm2m/text';
    }
    return ServerConfig;
}());
exports.ServerConfig = ServerConfig;
//# sourceMappingURL=config.js.map