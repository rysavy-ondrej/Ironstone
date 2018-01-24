// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

/**
 * The binary TLV (Type-Length-Value) format is used to represent an array of values  or a singular value using a compact binary representation, which is easy to process  on simple embedded devices. The format has a minimum overhead per value of just 2 bytes  and a maximum overhead of 5 bytes depending on the type of Identifier and length of the value.  The maximum size of an Object Instance or Resource in this format is 16.7 MB.  The format is self- describing, thus a parser can skip TLVs for which the Resource is not known. This data format has a Media Type of application/vnd.oma.lwm2m+tlv.
 */

var Lwm2mTlv = (function() {
  Lwm2mTlv.Lwm2mTlvIdentifierType = Object.freeze({
    OBJECT_INSTANCE: 0,
    RESOURCE_INSTANCE: 1,
    MULTIPLE_RESOURCE: 2,
    RESOURCE_WITH_VALUE: 3,

    0: "OBJECT_INSTANCE",
    1: "RESOURCE_INSTANCE",
    2: "MULTIPLE_RESOURCE",
    3: "RESOURCE_WITH_VALUE",
  });

  function Lwm2mTlv(_io, _parent, _root) {
    this._io = _io;
    this._parent = _parent;
    this._root = _root || this;

    this.type = new TlvType(this._io, this, this._root);
    this.identifier = new TlvIdentifier(this._io, this, this._root);
    this.length = new TlvLength(this._io, this, this._root);
    this.value = this._io.readBytes(this.length.value);
  }

  var TlvIdentifier = Lwm2mTlv.TlvIdentifier = (function() {
    function TlvIdentifier(_io, _parent, _root) {
      this._io = _io;
      this._parent = _parent;
      this._root = _root || this;

      if (this._parent.type.identifierWideLength == false) {
        this.tlvId1 = this._io.readU1();
      }
      if (this._parent.type.identifierWideLength == true) {
        this.tlvId2 = this._io.readU2be();
      }
    }
    Object.defineProperty(TlvIdentifier.prototype, 'value', {
      get: function() {
        if (this._m_value !== undefined)
          return this._m_value;
        this._m_value = (this.tlvId1 | this.tlvId2);
        return this._m_value;
      }
    });

    return TlvIdentifier;
  })();

  var TlvLength = Lwm2mTlv.TlvLength = (function() {
    function TlvLength(_io, _parent, _root) {
      this._io = _io;
      this._parent = _parent;
      this._root = _root || this;

      if (this._parent.type.lengthType == 1) {
        this.tlvLen1 = this._io.readU1();
      }
      if (this._parent.type.lengthType == 2) {
        this.tlvLen2 = this._io.readBitsInt(16);
      }
      if (this._parent.type.lengthType == 3) {
        this.tlvLen3 = this._io.readBitsInt(24);
      }
    }
    Object.defineProperty(TlvLength.prototype, 'value', {
      get: function() {
        if (this._m_value !== undefined)
          return this._m_value;
        this._m_value = (((this._parent.type.valueLength | this.tlvLen1) | this.tlvLen2) | this.tlvLen3);
        return this._m_value;
      }
    });

    return TlvLength;
  })();

  var TlvType = Lwm2mTlv.TlvType = (function() {
    function TlvType(_io, _parent, _root) {
      this._io = _io;
      this._parent = _parent;
      this._root = _root || this;

      this.identifierType = this._io.readBitsInt(2);
      this.identifierWideLength = this._io.readBitsInt(1) != 0;
      this.lengthType = this._io.readBitsInt(2);
      this.valueLength = this._io.readBitsInt(3);
    }

    /**
     * Bits 7-6: Indicates the type of Identifier:
     *   00= Object Instance in which case the Value contains one or more Resource TLVs
     *   01= Resource Instance with Value for use within a multiple Resource TLV
     *   10= multiple Resource, in which case the Value contains one or more Resource Instance TLVs
     *   11= Resource with Value
     */

    /**
     * Bit 5: Indicates the Length of the Identifier. 
     *   0=The Identifier field of this TLV is 8 bits long
     *   1=The Identifier field of this TLV is 16 bits long
     */

    /**
     * Bit 4-3: Indicates the type of Length.
     *   00 = No length field, the value immediately follows the Identifier field in is of the length indicated by Bits 2-0 of this field
     *   01 = The Length field is 8-bits and Bits 2-0 MUST be ignored
     *   10 = The Length field is 16-bits and Bits 2-0 MUST be ignored
     *   11 = The Length field is 24-bits and Bits 2-0 MUST be ignored
     */

    /**
     * Bits 2-0: A 3-bit unsigned integer indicating the Length of the Value.
     */

    return TlvType;
  })();

  /**
   * 8-bits masked field
   */

  /**
   * The Object Instance, Resource, or Resource Instance ID as indicated by the Type field.
   */

  /**
   * The Length of the following field in bytes.
   */

  /**
   * Value of the tag. The format of the value depends on the Resource‟s data type.
   */

  return Lwm2mTlv;
})();

// Export for amd environments
if (typeof define === 'function' && define.amd) {
  define('Lwm2mTlv', [], function() {
    return Lwm2mTlv;
  });
}

// Export for CommonJS
if (typeof module === 'object' && module && module.exports) {
  module.exports = Lwm2mTlv;
}
