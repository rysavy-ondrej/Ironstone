<?php
// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

/**
 * The binary TLV (Type-Length-Value) format is used to represent an array of values  or a singular value using a compact binary representation, which is easy to process  on simple embedded devices. The format has a minimum overhead per value of just 2 bytes  and a maximum overhead of 5 bytes depending on the type of Identifier and length of the value.  The maximum size of an Object Instance or Resource in this format is 16.7 MB.  The format is self- describing, thus a parser can skip TLVs for which the Resource is not known. This data format has a Media Type of application/vnd.oma.lwm2m+tlv.
 */

class Lwm2mTlv extends \Kaitai\Struct\Struct {

    public function __construct(\Kaitai\Struct\Stream $io, \Kaitai\Struct\Struct $parent = null, \Lwm2mTlv $root = null) {
        parent::__construct($io, $parent, $root);
        $this->_parse();
    }
    private function _parse() {
        $this->_m_type = new \Lwm2mTlv\TlvType($this->_io, $this, $this->_root);
        $this->_m_identifier = new \Lwm2mTlv\TlvIdentifier($this->_io, $this, $this->_root);
        $this->_m_length = new \Lwm2mTlv\TlvLength($this->_io, $this, $this->_root);
        $this->_m_value = $this->_io->readBytes($this->length()->value());
    }
    protected $_m_type;
    protected $_m_identifier;
    protected $_m_length;
    protected $_m_value;

    /**
     * 8-bits masked field
     */
    public function type() { return $this->_m_type; }

    /**
     * The Object Instance, Resource, or Resource Instance ID as indicated by the Type field.
     */
    public function identifier() { return $this->_m_identifier; }

    /**
     * The Length of the following field in bytes.
     */
    public function length() { return $this->_m_length; }

    /**
     * Value of the tag. The format of the value depends on the Resource‟s data type.
     */
    public function value() { return $this->_m_value; }
}

namespace \Lwm2mTlv;

class TlvIdentifier extends \Kaitai\Struct\Struct {

    public function __construct(\Kaitai\Struct\Stream $io, \Lwm2mTlv $parent = null, \Lwm2mTlv $root = null) {
        parent::__construct($io, $parent, $root);
        $this->_parse();
    }
    private function _parse() {
        if ($this->_parent()->type()->identifierWideLength() == false) {
            $this->_m_tlvId1 = $this->_io->readU1();
        }
        if ($this->_parent()->type()->identifierWideLength() == true) {
            $this->_m_tlvId2 = $this->_io->readU2be();
        }
    }
    protected $_m_value;
    public function value() {
        if ($this->_m_value !== null)
            return $this->_m_value;
        $this->_m_value = ($this->tlvId1() | $this->tlvId2());
        return $this->_m_value;
    }
    protected $_m_tlvId1;
    protected $_m_tlvId2;
    public function tlvId1() { return $this->_m_tlvId1; }
    public function tlvId2() { return $this->_m_tlvId2; }
}

namespace \Lwm2mTlv;

class TlvLength extends \Kaitai\Struct\Struct {

    public function __construct(\Kaitai\Struct\Stream $io, \Lwm2mTlv $parent = null, \Lwm2mTlv $root = null) {
        parent::__construct($io, $parent, $root);
        $this->_parse();
    }
    private function _parse() {
        if ($this->_parent()->type()->lengthType() == 1) {
            $this->_m_tlvLen1 = $this->_io->readU1();
        }
        if ($this->_parent()->type()->lengthType() == 2) {
            $this->_m_tlvLen2 = $this->_io->readBitsInt(16);
        }
        if ($this->_parent()->type()->lengthType() == 3) {
            $this->_m_tlvLen3 = $this->_io->readBitsInt(24);
        }
    }
    protected $_m_value;
    public function value() {
        if ($this->_m_value !== null)
            return $this->_m_value;
        $this->_m_value = ((($this->_parent()->type()->valueLength() | $this->tlvLen1()) | $this->tlvLen2()) | $this->tlvLen3());
        return $this->_m_value;
    }
    protected $_m_tlvLen1;
    protected $_m_tlvLen2;
    protected $_m_tlvLen3;
    public function tlvLen1() { return $this->_m_tlvLen1; }
    public function tlvLen2() { return $this->_m_tlvLen2; }
    public function tlvLen3() { return $this->_m_tlvLen3; }
}

namespace \Lwm2mTlv;

class TlvType extends \Kaitai\Struct\Struct {

    public function __construct(\Kaitai\Struct\Stream $io, \Lwm2mTlv $parent = null, \Lwm2mTlv $root = null) {
        parent::__construct($io, $parent, $root);
        $this->_parse();
    }
    private function _parse() {
        $this->_m_identifierType = $this->_io->readBitsInt(2);
        $this->_m_identifierWideLength = $this->_io->readBitsInt(1) != 0;
        $this->_m_lengthType = $this->_io->readBitsInt(2);
        $this->_m_valueLength = $this->_io->readBitsInt(3);
    }
    protected $_m_identifierType;
    protected $_m_identifierWideLength;
    protected $_m_lengthType;
    protected $_m_valueLength;

    /**
     * Bits 7-6: Indicates the type of Identifier:
     *   00= Object Instance in which case the Value contains one or more Resource TLVs
     *   01= Resource Instance with Value for use within a multiple Resource TLV
     *   10= multiple Resource, in which case the Value contains one or more Resource Instance TLVs
     *   11= Resource with Value
     */
    public function identifierType() { return $this->_m_identifierType; }

    /**
     * Bit 5: Indicates the Length of the Identifier. 
     *   0=The Identifier field of this TLV is 8 bits long
     *   1=The Identifier field of this TLV is 16 bits long
     */
    public function identifierWideLength() { return $this->_m_identifierWideLength; }

    /**
     * Bit 4-3: Indicates the type of Length.
     *   00 = No length field, the value immediately follows the Identifier field in is of the length indicated by Bits 2-0 of this field
     *   01 = The Length field is 8-bits and Bits 2-0 MUST be ignored
     *   10 = The Length field is 16-bits and Bits 2-0 MUST be ignored
     *   11 = The Length field is 24-bits and Bits 2-0 MUST be ignored
     */
    public function lengthType() { return $this->_m_lengthType; }

    /**
     * Bits 2-0: A 3-bit unsigned integer indicating the Length of the Value.
     */
    public function valueLength() { return $this->_m_valueLength; }
}

namespace \Lwm2mTlv;

class Lwm2mTlvIdentifierType {
    const OBJECT_INSTANCE = 0;
    const RESOURCE_INSTANCE = 1;
    const MULTIPLE_RESOURCE = 2;
    const RESOURCE_WITH_VALUE = 3;
}
