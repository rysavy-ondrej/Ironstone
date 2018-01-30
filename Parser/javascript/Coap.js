// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

var Coap = (function() {
  Coap.CoapMessageType = Object.freeze({
    CONFIRMABLE: 0,
    NON_COMFIRMANBLE: 1,
    ACKNOWLEDGEMENT: 2,
    RESET: 3,

    0: "CONFIRMABLE",
    1: "NON_COMFIRMANBLE",
    2: "ACKNOWLEDGEMENT",
    3: "RESET",
  });

  Coap.CoapCode = Object.freeze({
    EMPTY: 0,
    GET: 1,
    POST: 2,
    PUT: 3,
    DELETE: 4,
    CREATED: 65,
    DELETED: 66,
    VALID: 67,
    CHANGED: 68,
    CONTENT: 69,
    BAD_REQUEST: 128,
    UNATHORIZED: 129,
    BAD_OPTION: 130,
    FORBIDDEN: 131,
    NOT_FOUND: 132,
    METHOD_NOT_ALLOWED: 133,
    NOT_ACCEPTABLE: 134,
    PRECONDITION_FAILED: 140,
    REQUEST_ENTITY_TOO_LARGE: 141,
    UNSUPPORTED_CONTENT_FORMAT: 143,
    INTERNAL_SERVER_ERROR: 160,
    NOT_IMPLEMENTED: 161,
    BAD_GATEWAY: 162,
    SERVICE_UNAVAILABLE: 163,
    GATEWAY_TIMEOUT: 164,
    PROXYING_NOT_SUPPORTED: 165,

    0: "EMPTY",
    1: "GET",
    2: "POST",
    3: "PUT",
    4: "DELETE",
    65: "CREATED",
    66: "DELETED",
    67: "VALID",
    68: "CHANGED",
    69: "CONTENT",
    128: "BAD_REQUEST",
    129: "UNATHORIZED",
    130: "BAD_OPTION",
    131: "FORBIDDEN",
    132: "NOT_FOUND",
    133: "METHOD_NOT_ALLOWED",
    134: "NOT_ACCEPTABLE",
    140: "PRECONDITION_FAILED",
    141: "REQUEST_ENTITY_TOO_LARGE",
    143: "UNSUPPORTED_CONTENT_FORMAT",
    160: "INTERNAL_SERVER_ERROR",
    161: "NOT_IMPLEMENTED",
    162: "BAD_GATEWAY",
    163: "SERVICE_UNAVAILABLE",
    164: "GATEWAY_TIMEOUT",
    165: "PROXYING_NOT_SUPPORTED",
  });

  Coap.CoapOptions = Object.freeze({
    IF_MATCH: 1,
    URI_HOST: 3,
    ETAG: 4,
    IF_NONE_MATCH: 5,
    URI_PORT: 7,
    LOCATION_PATH: 8,
    URI_PATH: 11,
    CONTENT_FORMAT: 12,
    MAX_AGE: 14,
    URI_QUERY: 15,
    ACCEPT: 17,
    LOCATION_QUERY: 20,
    PROXY_URI: 35,
    PROXY_SCHEME: 39,
    SIZE1: 60,

    1: "IF_MATCH",
    3: "URI_HOST",
    4: "ETAG",
    5: "IF_NONE_MATCH",
    7: "URI_PORT",
    8: "LOCATION_PATH",
    11: "URI_PATH",
    12: "CONTENT_FORMAT",
    14: "MAX_AGE",
    15: "URI_QUERY",
    17: "ACCEPT",
    20: "LOCATION_QUERY",
    35: "PROXY_URI",
    39: "PROXY_SCHEME",
    60: "SIZE1",
  });

  function Coap(_io, _parent, _root) {
    this._io = _io;
    this._parent = _parent;
    this._root = _root || this;

    this.version = this._io.readBitsInt(2);
    this.type = this._io.readBitsInt(2);
    this.tkl = this._io.readBitsInt(4);
    this._io.alignToByte();
    this.code = this._io.readU1();
    this.messageId = this._io.readU2be();
    this.token = this._io.readBytes(this.tkl);
    if (this._io.isEof() == false) {
      this.options = []
      do {
        var _ = new Option(this._io, this, this._root);
        this.options.push(_);
      } while (!( ((_.isPayloadMarker) || (this._io.isEof())) ));
    }
    this.body = this._io.readBytesFull();
  }

  /**
   * Each option instance in a message specifies the Option Number of the defined CoAP option, the length of the Option Value, and the Option Value itself. Option nunber is expressed as delta. Both option length and delta values uses packing. Option is represented as  4 bits for regular values from 0-12. Values 13 and 14 informs that  option length is provided in extra bytes. The same holds for delta. 
   */

  var Option = Coap.Option = (function() {
    function Option(_io, _parent, _root) {
      this._io = _io;
      this._parent = _parent;
      this._root = _root || this;

      this.optDelta = this._io.readBitsInt(4);
      this.optLen = this._io.readBitsInt(4);
      this._io.alignToByte();
      if (this.optDelta == 13) {
        this.optDelta1 = this._io.readU1();
      }
      if (this.optDelta == 14) {
        this.optDelta2 = this._io.readU2be();
      }
      if (this.optLen == 13) {
        this.optLen1 = this._io.readU1();
      }
      if (this.optLen == 14) {
        this.optLen2 = this._io.readU2be();
      }
      this.value = this._io.readBytes(this.length);
    }
    Object.defineProperty(Option.prototype, 'length', {
      get: function() {
        if (this._m_length !== undefined)
          return this._m_length;
        this._m_length = (this.optLen == 13 ? this.optLen1 : (this.optLen == 14 ? this.optLen2 : (this.optLen == 15 ? 0 : this.optLen)));
        return this._m_length;
      }
    });
    Object.defineProperty(Option.prototype, 'delta', {
      get: function() {
        if (this._m_delta !== undefined)
          return this._m_delta;
        this._m_delta = (this.optDelta == 13 ? this.optDelta1 : (this.optDelta == 14 ? this.optDelta2 : (this.optDelta == 15 ? 0 : this.optDelta)));
        return this._m_delta;
      }
    });
    Object.defineProperty(Option.prototype, 'isPayloadMarker', {
      get: function() {
        if (this._m_isPayloadMarker !== undefined)
          return this._m_isPayloadMarker;
        this._m_isPayloadMarker =  ((this.optLen == 15) && (this.optDelta == 15)) ;
        return this._m_isPayloadMarker;
      }
    });

    return Option;
  })();

  return Coap;
})();

// Export for amd environments
if (typeof define === 'function' && define.amd) {
  define('Coap', [], function() {
    return Coap;
  });
}

// Export for CommonJS
if (typeof module === 'object' && module && module.exports) {
  module.exports = Coap;
}
