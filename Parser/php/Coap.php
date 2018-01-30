<?php
// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

class Coap extends \Kaitai\Struct\Struct {

    public function __construct(\Kaitai\Struct\Stream $io, \Kaitai\Struct\Struct $parent = null, \Coap $root = null) {
        parent::__construct($io, $parent, $root);
        $this->_parse();
    }
    private function _parse() {
        $this->_m_version = $this->_io->readBitsInt(2);
        $this->_m_type = $this->_io->readBitsInt(2);
        $this->_m_tkl = $this->_io->readBitsInt(4);
        $this->_io->alignToByte();
        $this->_m_code = $this->_io->readU1();
        $this->_m_messageId = $this->_io->readU2be();
        $this->_m_token = $this->_io->readBytes($this->tkl());
        if ($this->_io()->isEof() == false) {
            $this->_m_options = [];
            do {
                $_ = new \Coap\Option($this->_io, $this, $this->_root);
                $this->_m_options[] = $_;
            } while (!( (($_->isPayloadMarker()) || ($this->_io()->isEof())) ));
        }
        $this->_m_body = $this->_io->readBytesFull();
    }
    protected $_m_version;
    protected $_m_type;
    protected $_m_tkl;
    protected $_m_code;
    protected $_m_messageId;
    protected $_m_token;
    protected $_m_options;
    protected $_m_body;
    public function version() { return $this->_m_version; }
    public function type() { return $this->_m_type; }
    public function tkl() { return $this->_m_tkl; }
    public function code() { return $this->_m_code; }
    public function messageId() { return $this->_m_messageId; }
    public function token() { return $this->_m_token; }
    public function options() { return $this->_m_options; }
    public function body() { return $this->_m_body; }
}

/**
 * Each option instance in a message specifies the Option Number of the defined CoAP option, the length of the Option Value, and the Option Value itself. Option nunber is expressed as delta. Both option length and delta values uses packing. Option is represented as  4 bits for regular values from 0-12. Values 13 and 14 informs that  option length is provided in extra bytes. The same holds for delta. 
 */

namespace \Coap;

class Option extends \Kaitai\Struct\Struct {

    public function __construct(\Kaitai\Struct\Stream $io, \Coap $parent = null, \Coap $root = null) {
        parent::__construct($io, $parent, $root);
        $this->_parse();
    }
    private function _parse() {
        $this->_m_optDelta = $this->_io->readBitsInt(4);
        $this->_m_optLen = $this->_io->readBitsInt(4);
        $this->_io->alignToByte();
        if ($this->optDelta() == 13) {
            $this->_m_optDelta1 = $this->_io->readU1();
        }
        if ($this->optDelta() == 14) {
            $this->_m_optDelta2 = $this->_io->readU2be();
        }
        if ($this->optLen() == 13) {
            $this->_m_optLen1 = $this->_io->readU1();
        }
        if ($this->optLen() == 14) {
            $this->_m_optLen2 = $this->_io->readU2be();
        }
        $this->_m_value = $this->_io->readBytes($this->length());
    }
    protected $_m_length;
    public function length() {
        if ($this->_m_length !== null)
            return $this->_m_length;
        $this->_m_length = ($this->optLen() == 13 ? $this->optLen1() : ($this->optLen() == 14 ? $this->optLen2() : ($this->optLen() == 15 ? 0 : $this->optLen())));
        return $this->_m_length;
    }
    protected $_m_delta;
    public function delta() {
        if ($this->_m_delta !== null)
            return $this->_m_delta;
        $this->_m_delta = ($this->optDelta() == 13 ? $this->optDelta1() : ($this->optDelta() == 14 ? $this->optDelta2() : ($this->optDelta() == 15 ? 0 : $this->optDelta())));
        return $this->_m_delta;
    }
    protected $_m_isPayloadMarker;
    public function isPayloadMarker() {
        if ($this->_m_isPayloadMarker !== null)
            return $this->_m_isPayloadMarker;
        $this->_m_isPayloadMarker =  (($this->optLen() == 15) && ($this->optDelta() == 15)) ;
        return $this->_m_isPayloadMarker;
    }
    protected $_m_optDelta;
    protected $_m_optLen;
    protected $_m_optDelta1;
    protected $_m_optDelta2;
    protected $_m_optLen1;
    protected $_m_optLen2;
    protected $_m_value;
    public function optDelta() { return $this->_m_optDelta; }
    public function optLen() { return $this->_m_optLen; }
    public function optDelta1() { return $this->_m_optDelta1; }
    public function optDelta2() { return $this->_m_optDelta2; }
    public function optLen1() { return $this->_m_optLen1; }
    public function optLen2() { return $this->_m_optLen2; }
    public function value() { return $this->_m_value; }
}

namespace \Coap;

class CoapMessageType {
    const CONFIRMABLE = 0;
    const NON_COMFIRMANBLE = 1;
    const ACKNOWLEDGEMENT = 2;
    const RESET = 3;
}

namespace \Coap;

class CoapCode {
    const EMPTY = 0;
    const GET = 1;
    const POST = 2;
    const PUT = 3;
    const DELETE = 4;
    const CREATED = 65;
    const DELETED = 66;
    const VALID = 67;
    const CHANGED = 68;
    const CONTENT = 69;
    const BAD_REQUEST = 128;
    const UNATHORIZED = 129;
    const BAD_OPTION = 130;
    const FORBIDDEN = 131;
    const NOT_FOUND = 132;
    const METHOD_NOT_ALLOWED = 133;
    const NOT_ACCEPTABLE = 134;
    const PRECONDITION_FAILED = 140;
    const REQUEST_ENTITY_TOO_LARGE = 141;
    const UNSUPPORTED_CONTENT_FORMAT = 143;
    const INTERNAL_SERVER_ERROR = 160;
    const NOT_IMPLEMENTED = 161;
    const BAD_GATEWAY = 162;
    const SERVICE_UNAVAILABLE = 163;
    const GATEWAY_TIMEOUT = 164;
    const PROXYING_NOT_SUPPORTED = 165;
}

namespace \Coap;

class CoapOptions {
    const IF_MATCH = 1;
    const URI_HOST = 3;
    const ETAG = 4;
    const IF_NONE_MATCH = 5;
    const URI_PORT = 7;
    const LOCATION_PATH = 8;
    const URI_PATH = 11;
    const CONTENT_FORMAT = 12;
    const MAX_AGE = 14;
    const URI_QUERY = 15;
    const ACCEPT = 17;
    const LOCATION_QUERY = 20;
    const PROXY_URI = 35;
    const PROXY_SCHEME = 39;
    const SIZE1 = 60;
}
