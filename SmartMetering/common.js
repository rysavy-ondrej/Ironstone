"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var Common = /** @class */ (function () {
    function Common() {
    }
    Common.getTs = function () {
        return new Date().toLocaleString('en-US');
    };
    Common.precisionRound = function (number, precision) {
        var factor = Math.pow(10, precision);
        return Math.round(number * factor) / factor;
    };
    Common.randomIntInc = function (low, high) {
        return Math.random() * (high - low + 1) + low;
    };
    return Common;
}());
exports.Common = Common;
//# sourceMappingURL=common.js.map