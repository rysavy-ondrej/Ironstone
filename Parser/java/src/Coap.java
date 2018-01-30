// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

import io.kaitai.struct.KaitaiStruct;
import io.kaitai.struct.KaitaiStream;

import java.io.IOException;
import java.util.Arrays;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;
import java.nio.charset.Charset;

public class Coap extends KaitaiStruct {
    public static Coap fromFile(String fileName) throws IOException {
        return new Coap(new KaitaiStream(fileName));
    }

    public enum CoapMessageType {
        CONFIRMABLE(0),
        NON_COMFIRMANBLE(1),
        ACKNOWLEDGEMENT(2),
        RESET(3);

        private final long id;
        CoapMessageType(long id) { this.id = id; }
        public long id() { return id; }
        private static final Map<Long, CoapMessageType> byId = new HashMap<Long, CoapMessageType>(4);
        static {
            for (CoapMessageType e : CoapMessageType.values())
                byId.put(e.id(), e);
        }
        public static CoapMessageType byId(long id) { return byId.get(id); }
    }

    public enum CoapCode {
        EMPTY(0),
        GET(1),
        POST(2),
        PUT(3),
        DELETE(4),
        CREATED(65),
        DELETED(66),
        VALID(67),
        CHANGED(68),
        CONTENT(69),
        BAD_REQUEST(128),
        UNATHORIZED(129),
        BAD_OPTION(130),
        FORBIDDEN(131),
        NOT_FOUND(132),
        METHOD_NOT_ALLOWED(133),
        NOT_ACCEPTABLE(134),
        PRECONDITION_FAILED(140),
        REQUEST_ENTITY_TOO_LARGE(141),
        UNSUPPORTED_CONTENT_FORMAT(143),
        INTERNAL_SERVER_ERROR(160),
        NOT_IMPLEMENTED(161),
        BAD_GATEWAY(162),
        SERVICE_UNAVAILABLE(163),
        GATEWAY_TIMEOUT(164),
        PROXYING_NOT_SUPPORTED(165);

        private final long id;
        CoapCode(long id) { this.id = id; }
        public long id() { return id; }
        private static final Map<Long, CoapCode> byId = new HashMap<Long, CoapCode>(26);
        static {
            for (CoapCode e : CoapCode.values())
                byId.put(e.id(), e);
        }
        public static CoapCode byId(long id) { return byId.get(id); }
    }

    public enum CoapOptions {
        IF_MATCH(1),
        URI_HOST(3),
        ETAG(4),
        IF_NONE_MATCH(5),
        URI_PORT(7),
        LOCATION_PATH(8),
        URI_PATH(11),
        CONTENT_FORMAT(12),
        MAX_AGE(14),
        URI_QUERY(15),
        ACCEPT(17),
        LOCATION_QUERY(20),
        PROXY_URI(35),
        PROXY_SCHEME(39),
        SIZE1(60);

        private final long id;
        CoapOptions(long id) { this.id = id; }
        public long id() { return id; }
        private static final Map<Long, CoapOptions> byId = new HashMap<Long, CoapOptions>(15);
        static {
            for (CoapOptions e : CoapOptions.values())
                byId.put(e.id(), e);
        }
        public static CoapOptions byId(long id) { return byId.get(id); }
    }

    public Coap(KaitaiStream _io) {
        super(_io);
        this._root = this;
        _read();
    }

    public Coap(KaitaiStream _io, KaitaiStruct _parent) {
        super(_io);
        this._parent = _parent;
        this._root = this;
        _read();
    }

    public Coap(KaitaiStream _io, KaitaiStruct _parent, Coap _root) {
        super(_io);
        this._parent = _parent;
        this._root = _root;
        _read();
    }
    private void _read() {
        this.version = this._io.readBitsInt(2);
        this.type = CoapMessageType.byId(this._io.readBitsInt(2));
        this.tkl = this._io.readBitsInt(4);
        this._io.alignToByte();
        this.code = CoapCode.byId(this._io.readU1());
        this.messageId = this._io.readU2be();
        this.token = this._io.readBytes(tkl());
        if (_io().isEof() == false) {
            this.options = new ArrayList<Option>();
            {
                Option _it;
                do {
                    _it = new Option(this._io, this, _root);
                    this.options.add(_it);
                } while (!( ((_it.isPayloadMarker()) || (_io().isEof())) ));
            }
        }
        this.body = this._io.readBytesFull();
    }

    /**
     * Each option instance in a message specifies the Option Number of the defined CoAP option, the length of the Option Value, and the Option Value itself. Option nunber is expressed as delta. Both option length and delta values uses packing. Option is represented as  4 bits for regular values from 0-12. Values 13 and 14 informs that  option length is provided in extra bytes. The same holds for delta. 
     */
    public static class Option extends KaitaiStruct {
        public static Option fromFile(String fileName) throws IOException {
            return new Option(new KaitaiStream(fileName));
        }

        public Option(KaitaiStream _io) {
            super(_io);
            _read();
        }

        public Option(KaitaiStream _io, Coap _parent) {
            super(_io);
            this._parent = _parent;
            _read();
        }

        public Option(KaitaiStream _io, Coap _parent, Coap _root) {
            super(_io);
            this._parent = _parent;
            this._root = _root;
            _read();
        }
        private void _read() {
            this.optDelta = this._io.readBitsInt(4);
            this.optLen = this._io.readBitsInt(4);
            this._io.alignToByte();
            if (optDelta() == 13) {
                this.optDelta1 = this._io.readU1();
            }
            if (optDelta() == 14) {
                this.optDelta2 = this._io.readU2be();
            }
            if (optLen() == 13) {
                this.optLen1 = this._io.readU1();
            }
            if (optLen() == 14) {
                this.optLen2 = this._io.readU2be();
            }
            this.value = this._io.readBytes(length());
        }
        private Integer length;
        public Integer length() {
            if (this.length != null)
                return this.length;
            int _tmp = (int) ((optLen() == 13 ? optLen1() : (optLen() == 14 ? optLen2() : (optLen() == 15 ? 0 : optLen()))));
            this.length = _tmp;
            return this.length;
        }
        private Integer delta;
        public Integer delta() {
            if (this.delta != null)
                return this.delta;
            int _tmp = (int) ((optDelta() == 13 ? optDelta1() : (optDelta() == 14 ? optDelta2() : (optDelta() == 15 ? 0 : optDelta()))));
            this.delta = _tmp;
            return this.delta;
        }
        private Boolean isPayloadMarker;
        public Boolean isPayloadMarker() {
            if (this.isPayloadMarker != null)
                return this.isPayloadMarker;
            boolean _tmp = (boolean) ( ((optLen() == 15) && (optDelta() == 15)) );
            this.isPayloadMarker = _tmp;
            return this.isPayloadMarker;
        }
        private long optDelta;
        private long optLen;
        private Integer optDelta1;
        private Integer optDelta2;
        private Integer optLen1;
        private Integer optLen2;
        private byte[] value;
        private Coap _root;
        private Coap _parent;
        public long optDelta() { return optDelta; }
        public long optLen() { return optLen; }
        public Integer optDelta1() { return optDelta1; }
        public Integer optDelta2() { return optDelta2; }
        public Integer optLen1() { return optLen1; }
        public Integer optLen2() { return optLen2; }
        public byte[] value() { return value; }
        public Coap _root() { return _root; }
        public Coap _parent() { return _parent; }
    }
    private long version;
    private CoapMessageType type;
    private long tkl;
    private CoapCode code;
    private int messageId;
    private byte[] token;
    private ArrayList<Option> options;
    private byte[] body;
    private Coap _root;
    private KaitaiStruct _parent;
    public long version() { return version; }
    public CoapMessageType type() { return type; }
    public long tkl() { return tkl; }
    public CoapCode code() { return code; }
    public int messageId() { return messageId; }
    public byte[] token() { return token; }
    public ArrayList<Option> options() { return options; }
    public byte[] body() { return body; }
    public Coap _root() { return _root; }
    public KaitaiStruct _parent() { return _parent; }
}
