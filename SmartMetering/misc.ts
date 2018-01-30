export class Misc {
    public static getTs() : string {
        return new Date().toLocaleString('en-US');
    }

    public static precisionRound(number : number, precision : number) : number {
        var factor = Math.pow(10, precision);
        return Math.round(number * factor) / factor;
    }

    public static randomIntInc (low : number, high : number) : number {
        return Math.random() * (high - low + 1) + low;
    }

    public static logEvent(message : string) {
        console.log('[EVENT: ts=%s] %s', Misc.getTs(), message);
    }
}
