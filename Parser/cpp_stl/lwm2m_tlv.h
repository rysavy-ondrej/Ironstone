#ifndef LWM2M_TLV_H_
#define LWM2M_TLV_H_

// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

#include <kaitai/kaitaistruct.h>
#include <kaitai/kaitaistream.h>

#include <stdint.h>
#include <vector>
#include <sstream>

#if KAITAI_STRUCT_VERSION < 7000L
#error "Incompatible Kaitai Struct C++/STL API: version 0.7 or later is required"
#endif

/**
 * The binary TLV (Type-Length-Value) format is used to represent an array of values  or a singular value using a compact binary representation, which is easy to process  on simple embedded devices. The format has a minimum overhead per value of just 2 bytes  and a maximum overhead of 5 bytes depending on the type of Identifier and length of the value.  The maximum size of an Object Instance or Resource in this format is 16.7 MB.  The format is self- describing, thus a parser can skip TLVs for which the Resource is not known. This data format has a Media Type of application/vnd.oma.lwm2m+tlv.
 */

class lwm2m_tlv_t : public kaitai::kstruct {

public:
    class tlv_identifier_t;
    class tlv_length_t;
    class tlv_type_t;

    enum lwm2m_tlv_identifier_type_t {
        LWM2M_TLV_IDENTIFIER_TYPE_OBJECT_INSTANCE = 0,
        LWM2M_TLV_IDENTIFIER_TYPE_RESOURCE_INSTANCE = 1,
        LWM2M_TLV_IDENTIFIER_TYPE_MULTIPLE_RESOURCE = 2,
        LWM2M_TLV_IDENTIFIER_TYPE_RESOURCE_WITH_VALUE = 3
    };

    lwm2m_tlv_t(kaitai::kstream* p_io, kaitai::kstruct* p_parent = 0, lwm2m_tlv_t* p_root = 0);
    ~lwm2m_tlv_t();

    class tlv_identifier_t : public kaitai::kstruct {

    public:

        tlv_identifier_t(kaitai::kstream* p_io, lwm2m_tlv_t* p_parent = 0, lwm2m_tlv_t* p_root = 0);
        ~tlv_identifier_t();

    private:
        bool f_value;
        int32_t m_value;

    public:
        int32_t value();

    private:
        uint8_t m_tlv_id_1;
        bool n_tlv_id_1;

    public:
        bool _is_null_tlv_id_1() { tlv_id_1(); return n_tlv_id_1; };

    private:
        uint16_t m_tlv_id_2;
        bool n_tlv_id_2;

    public:
        bool _is_null_tlv_id_2() { tlv_id_2(); return n_tlv_id_2; };

    private:
        lwm2m_tlv_t* m__root;
        lwm2m_tlv_t* m__parent;

    public:
        uint8_t tlv_id_1() const { return m_tlv_id_1; }
        uint16_t tlv_id_2() const { return m_tlv_id_2; }
        lwm2m_tlv_t* _root() const { return m__root; }
        lwm2m_tlv_t* _parent() const { return m__parent; }
    };

    class tlv_length_t : public kaitai::kstruct {

    public:

        tlv_length_t(kaitai::kstream* p_io, lwm2m_tlv_t* p_parent = 0, lwm2m_tlv_t* p_root = 0);
        ~tlv_length_t();

    private:
        bool f_value;
        int32_t m_value;

    public:
        int32_t value();

    private:
        uint8_t m_tlv_len_1;
        bool n_tlv_len_1;

    public:
        bool _is_null_tlv_len_1() { tlv_len_1(); return n_tlv_len_1; };

    private:
        uint64_t m_tlv_len_2;
        bool n_tlv_len_2;

    public:
        bool _is_null_tlv_len_2() { tlv_len_2(); return n_tlv_len_2; };

    private:
        uint64_t m_tlv_len_3;
        bool n_tlv_len_3;

    public:
        bool _is_null_tlv_len_3() { tlv_len_3(); return n_tlv_len_3; };

    private:
        lwm2m_tlv_t* m__root;
        lwm2m_tlv_t* m__parent;

    public:
        uint8_t tlv_len_1() const { return m_tlv_len_1; }
        uint64_t tlv_len_2() const { return m_tlv_len_2; }
        uint64_t tlv_len_3() const { return m_tlv_len_3; }
        lwm2m_tlv_t* _root() const { return m__root; }
        lwm2m_tlv_t* _parent() const { return m__parent; }
    };

    class tlv_type_t : public kaitai::kstruct {

    public:

        tlv_type_t(kaitai::kstream* p_io, lwm2m_tlv_t* p_parent = 0, lwm2m_tlv_t* p_root = 0);
        ~tlv_type_t();

    private:
        lwm2m_tlv_identifier_type_t m_identifier_type;
        bool m_identifier_wide_length;
        uint64_t m_length_type;
        uint64_t m_value_length;
        lwm2m_tlv_t* m__root;
        lwm2m_tlv_t* m__parent;

    public:

        /**
         * Bits 7-6: Indicates the type of Identifier:
         *   00= Object Instance in which case the Value contains one or more Resource TLVs
         *   01= Resource Instance with Value for use within a multiple Resource TLV
         *   10= multiple Resource, in which case the Value contains one or more Resource Instance TLVs
         *   11= Resource with Value
         */
        lwm2m_tlv_identifier_type_t identifier_type() const { return m_identifier_type; }

        /**
         * Bit 5: Indicates the Length of the Identifier. 
         *   0=The Identifier field of this TLV is 8 bits long
         *   1=The Identifier field of this TLV is 16 bits long
         */
        bool identifier_wide_length() const { return m_identifier_wide_length; }

        /**
         * Bit 4-3: Indicates the type of Length.
         *   00 = No length field, the value immediately follows the Identifier field in is of the length indicated by Bits 2-0 of this field
         *   01 = The Length field is 8-bits and Bits 2-0 MUST be ignored
         *   10 = The Length field is 16-bits and Bits 2-0 MUST be ignored
         *   11 = The Length field is 24-bits and Bits 2-0 MUST be ignored
         */
        uint64_t length_type() const { return m_length_type; }

        /**
         * Bits 2-0: A 3-bit unsigned integer indicating the Length of the Value.
         */
        uint64_t value_length() const { return m_value_length; }
        lwm2m_tlv_t* _root() const { return m__root; }
        lwm2m_tlv_t* _parent() const { return m__parent; }
    };

private:
    tlv_type_t* m_type;
    tlv_identifier_t* m_identifier;
    tlv_length_t* m_length;
    std::string m_value;
    lwm2m_tlv_t* m__root;
    kaitai::kstruct* m__parent;

public:

    /**
     * 8-bits masked field
     */
    tlv_type_t* type() const { return m_type; }

    /**
     * The Object Instance, Resource, or Resource Instance ID as indicated by the Type field.
     */
    tlv_identifier_t* identifier() const { return m_identifier; }

    /**
     * The Length of the following field in bytes.
     */
    tlv_length_t* length() const { return m_length; }

    /**
     * Value of the tag. The format of the value depends on the Resource‟s data type.
     */
    std::string value() const { return m_value; }
    lwm2m_tlv_t* _root() const { return m__root; }
    kaitai::kstruct* _parent() const { return m__parent; }
};

#endif  // LWM2M_TLV_H_
