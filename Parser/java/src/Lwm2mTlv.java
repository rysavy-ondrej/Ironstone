// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

import io.kaitai.struct.KaitaiStruct;
import io.kaitai.struct.KaitaiStream;

import java.io.IOException;
import java.util.Arrays;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Map;
import java.nio.charset.Charset;


/**
 * The binary TLV (Type-Length-Value) format is used to represent an array of values  or a singular value using a compact binary representation, which is easy to process  on simple embedded devices. The format has a minimum overhead per value of just 2 bytes  and a maximum overhead of 5 bytes depending on the type of Identifier and length of the value.  The maximum size of an Object Instance or Resource in this format is 16.7 MB.  The format is self- describing, thus a parser can skip TLVs for which the Resource is not known. This data format has a Media Type of application/vnd.oma.lwm2m+tlv.
 */
public class Lwm2mTlv extends KaitaiStruct {
    public static Lwm2mTlv fromFile(String fileName) throws IOException {
        return new Lwm2mTlv(new KaitaiStream(fileName));
    }

    public enum Lwm2mTlvIdentifierType {
        OBJECT_INSTANCE(0),
        RESOURCE_INSTANCE(1),
        MULTIPLE_RESOURCE(2),
        RESOURCE_WITH_VALUE(3);

        private final long id;
        Lwm2mTlvIdentifierType(long id) { this.id = id; }
        public long id() { return id; }
        private static final Map<Long, Lwm2mTlvIdentifierType> byId = new HashMap<Long, Lwm2mTlvIdentifierType>(4);
        static {
            for (Lwm2mTlvIdentifierType e : Lwm2mTlvIdentifierType.values())
                byId.put(e.id(), e);
        }
        public static Lwm2mTlvIdentifierType byId(long id) { return byId.get(id); }
    }

    public Lwm2mTlv(KaitaiStream _io) {
        super(_io);
        this._root = this;
        _read();
    }

    public Lwm2mTlv(KaitaiStream _io, KaitaiStruct _parent) {
        super(_io);
        this._parent = _parent;
        this._root = this;
        _read();
    }

    public Lwm2mTlv(KaitaiStream _io, KaitaiStruct _parent, Lwm2mTlv _root) {
        super(_io);
        this._parent = _parent;
        this._root = _root;
        _read();
    }
    private void _read() {
        this.type = new TlvType(this._io, this, _root);
        this.identifier = new TlvIdentifier(this._io, this, _root);
        this.length = new TlvLength(this._io, this, _root);
        this.value = this._io.readBytes(length().value());
    }
    public static class TlvIdentifier extends KaitaiStruct {
        public static TlvIdentifier fromFile(String fileName) throws IOException {
            return new TlvIdentifier(new KaitaiStream(fileName));
        }

        public TlvIdentifier(KaitaiStream _io) {
            super(_io);
            _read();
        }

        public TlvIdentifier(KaitaiStream _io, Lwm2mTlv _parent) {
            super(_io);
            this._parent = _parent;
            _read();
        }

        public TlvIdentifier(KaitaiStream _io, Lwm2mTlv _parent, Lwm2mTlv _root) {
            super(_io);
            this._parent = _parent;
            this._root = _root;
            _read();
        }
        private void _read() {
            if (_parent().type().identifierWideLength() == false) {
                this.tlvId1 = this._io.readU1();
            }
            if (_parent().type().identifierWideLength() == true) {
                this.tlvId2 = this._io.readU2be();
            }
        }
        private Integer value;
        public Integer value() {
            if (this.value != null)
                return this.value;
            int _tmp = (int) ((tlvId1() | tlvId2()));
            this.value = _tmp;
            return this.value;
        }
        private Integer tlvId1;
        private Integer tlvId2;
        private Lwm2mTlv _root;
        private Lwm2mTlv _parent;
        public Integer tlvId1() { return tlvId1; }
        public Integer tlvId2() { return tlvId2; }
        public Lwm2mTlv _root() { return _root; }
        public Lwm2mTlv _parent() { return _parent; }
    }
    public static class TlvLength extends KaitaiStruct {
        public static TlvLength fromFile(String fileName) throws IOException {
            return new TlvLength(new KaitaiStream(fileName));
        }

        public TlvLength(KaitaiStream _io) {
            super(_io);
            _read();
        }

        public TlvLength(KaitaiStream _io, Lwm2mTlv _parent) {
            super(_io);
            this._parent = _parent;
            _read();
        }

        public TlvLength(KaitaiStream _io, Lwm2mTlv _parent, Lwm2mTlv _root) {
            super(_io);
            this._parent = _parent;
            this._root = _root;
            _read();
        }
        private void _read() {
            if (_parent().type().lengthType() == 1) {
                this.tlvLen1 = this._io.readU1();
            }
            if (_parent().type().lengthType() == 2) {
                this.tlvLen2 = this._io.readBitsInt(16);
            }
            if (_parent().type().lengthType() == 3) {
                this.tlvLen3 = this._io.readBitsInt(24);
            }
        }
        private Integer value;
        public Integer value() {
            if (this.value != null)
                return this.value;
            int _tmp = (int) ((((_parent().type().valueLength() | tlvLen1()) | tlvLen2()) | tlvLen3()));
            this.value = _tmp;
            return this.value;
        }
        private Integer tlvLen1;
        private Long tlvLen2;
        private Long tlvLen3;
        private Lwm2mTlv _root;
        private Lwm2mTlv _parent;
        public Integer tlvLen1() { return tlvLen1; }
        public Long tlvLen2() { return tlvLen2; }
        public Long tlvLen3() { return tlvLen3; }
        public Lwm2mTlv _root() { return _root; }
        public Lwm2mTlv _parent() { return _parent; }
    }
    public static class TlvType extends KaitaiStruct {
        public static TlvType fromFile(String fileName) throws IOException {
            return new TlvType(new KaitaiStream(fileName));
        }

        public TlvType(KaitaiStream _io) {
            super(_io);
            _read();
        }

        public TlvType(KaitaiStream _io, Lwm2mTlv _parent) {
            super(_io);
            this._parent = _parent;
            _read();
        }

        public TlvType(KaitaiStream _io, Lwm2mTlv _parent, Lwm2mTlv _root) {
            super(_io);
            this._parent = _parent;
            this._root = _root;
            _read();
        }
        private void _read() {
            this.identifierType = Lwm2mTlv.Lwm2mTlvIdentifierType.byId(this._io.readBitsInt(2));
            this.identifierWideLength = this._io.readBitsInt(1) != 0;
            this.lengthType = this._io.readBitsInt(2);
            this.valueLength = this._io.readBitsInt(3);
        }
        private Lwm2mTlvIdentifierType identifierType;
        private boolean identifierWideLength;
        private long lengthType;
        private long valueLength;
        private Lwm2mTlv _root;
        private Lwm2mTlv _parent;

        /**
         * Bits 7-6: Indicates the type of Identifier:
         *   00= Object Instance in which case the Value contains one or more Resource TLVs
         *   01= Resource Instance with Value for use within a multiple Resource TLV
         *   10= multiple Resource, in which case the Value contains one or more Resource Instance TLVs
         *   11= Resource with Value
         */
        public Lwm2mTlvIdentifierType identifierType() { return identifierType; }

        /**
         * Bit 5: Indicates the Length of the Identifier. 
         *   0=The Identifier field of this TLV is 8 bits long
         *   1=The Identifier field of this TLV is 16 bits long
         */
        public boolean identifierWideLength() { return identifierWideLength; }

        /**
         * Bit 4-3: Indicates the type of Length.
         *   00 = No length field, the value immediately follows the Identifier field in is of the length indicated by Bits 2-0 of this field
         *   01 = The Length field is 8-bits and Bits 2-0 MUST be ignored
         *   10 = The Length field is 16-bits and Bits 2-0 MUST be ignored
         *   11 = The Length field is 24-bits and Bits 2-0 MUST be ignored
         */
        public long lengthType() { return lengthType; }

        /**
         * Bits 2-0: A 3-bit unsigned integer indicating the Length of the Value.
         */
        public long valueLength() { return valueLength; }
        public Lwm2mTlv _root() { return _root; }
        public Lwm2mTlv _parent() { return _parent; }
    }
    private TlvType type;
    private TlvIdentifier identifier;
    private TlvLength length;
    private byte[] value;
    private Lwm2mTlv _root;
    private KaitaiStruct _parent;

    /**
     * 8-bits masked field
     */
    public TlvType type() { return type; }

    /**
     * The Object Instance, Resource, or Resource Instance ID as indicated by the Type field.
     */
    public TlvIdentifier identifier() { return identifier; }

    /**
     * The Length of the following field in bytes.
     */
    public TlvLength length() { return length; }

    /**
     * Value of the tag. The format of the value depends on the Resource‟s data type.
     */
    public byte[] value() { return value; }
    public Lwm2mTlv _root() { return _root; }
    public KaitaiStruct _parent() { return _parent; }
}
