"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var Misc = /** @class */ (function () {
    function Misc() {
    }
    Misc.getTs = function () {
        return new Date().toLocaleString('en-US');
    };
    Misc.precisionRound = function (number, precision) {
        var factor = Math.pow(10, precision);
        return Math.round(number * factor) / factor;
    };
    Misc.randomIntInc = function (low, high) {
        return Math.random() * (high - low + 1) + low;
    };
    Misc.logEvent = function (message) {
        console.log('[EVENT: ts=%s] %s', Misc.getTs(), message);
    };
    return Misc;
}());
exports.Misc = Misc;
//# sourceMappingURL=misc.js.map