#ifndef COAP_H_
#define COAP_H_

// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

#include <kaitai/kaitaistruct.h>
#include <kaitai/kaitaistream.h>

#include <stdint.h>
#include <vector>
#include <sstream>

#if KAITAI_STRUCT_VERSION < 7000L
#error "Incompatible Kaitai Struct C++/STL API: version 0.7 or later is required"
#endif

class coap_t : public kaitai::kstruct {

public:
    class option_t;

    enum coap_message_type_t {
        COAP_MESSAGE_TYPE_CONFIRMABLE = 0,
        COAP_MESSAGE_TYPE_NON_COMFIRMANBLE = 1,
        COAP_MESSAGE_TYPE_ACKNOWLEDGEMENT = 2,
        COAP_MESSAGE_TYPE_RESET = 3
    };

    enum coap_code_t {
        COAP_CODE_EMPTY = 0,
        COAP_CODE_GET = 1,
        COAP_CODE_POST = 2,
        COAP_CODE_PUT = 3,
        COAP_CODE_DELETE = 4,
        COAP_CODE_CREATED = 65,
        COAP_CODE_DELETED = 66,
        COAP_CODE_VALID = 67,
        COAP_CODE_CHANGED = 68,
        COAP_CODE_CONTENT = 69,
        COAP_CODE_BAD_REQUEST = 128,
        COAP_CODE_UNATHORIZED = 129,
        COAP_CODE_BAD_OPTION = 130,
        COAP_CODE_FORBIDDEN = 131,
        COAP_CODE_NOT_FOUND = 132,
        COAP_CODE_METHOD_NOT_ALLOWED = 133,
        COAP_CODE_NOT_ACCEPTABLE = 134,
        COAP_CODE_PRECONDITION_FAILED = 140,
        COAP_CODE_REQUEST_ENTITY_TOO_LARGE = 141,
        COAP_CODE_UNSUPPORTED_CONTENT_FORMAT = 143,
        COAP_CODE_INTERNAL_SERVER_ERROR = 160,
        COAP_CODE_NOT_IMPLEMENTED = 161,
        COAP_CODE_BAD_GATEWAY = 162,
        COAP_CODE_SERVICE_UNAVAILABLE = 163,
        COAP_CODE_GATEWAY_TIMEOUT = 164,
        COAP_CODE_PROXYING_NOT_SUPPORTED = 165
    };

    enum coap_options_t {
        COAP_OPTIONS_IF_MATCH = 1,
        COAP_OPTIONS_URI_HOST = 3,
        COAP_OPTIONS_ETAG = 4,
        COAP_OPTIONS_IF_NONE_MATCH = 5,
        COAP_OPTIONS_URI_PORT = 7,
        COAP_OPTIONS_LOCATION_PATH = 8,
        COAP_OPTIONS_URI_PATH = 11,
        COAP_OPTIONS_CONTENT_FORMAT = 12,
        COAP_OPTIONS_MAX_AGE = 14,
        COAP_OPTIONS_URI_QUERY = 15,
        COAP_OPTIONS_ACCEPT = 17,
        COAP_OPTIONS_LOCATION_QUERY = 20,
        COAP_OPTIONS_PROXY_URI = 35,
        COAP_OPTIONS_PROXY_SCHEME = 39,
        COAP_OPTIONS_SIZE1 = 60
    };

    coap_t(kaitai::kstream* p_io, kaitai::kstruct* p_parent = 0, coap_t* p_root = 0);
    ~coap_t();

    /**
     * Each option instance in a message specifies the Option Number of the defined CoAP option, the length of the Option Value, and the Option Value itself. Option nunber is expressed as delta. Both option length and delta values uses packing. Option is represented as  4 bits for regular values from 0-12. Values 13 and 14 informs that  option length is provided in extra bytes. The same holds for delta. 
     */

    class option_t : public kaitai::kstruct {

    public:

        option_t(kaitai::kstream* p_io, coap_t* p_parent = 0, coap_t* p_root = 0);
        ~option_t();

    private:
        bool f_length;
        int32_t m_length;

    public:
        int32_t length();

    private:
        bool f_delta;
        int32_t m_delta;

    public:
        int32_t delta();

    private:
        bool f_is_payload_marker;
        bool m_is_payload_marker;

    public:
        bool is_payload_marker();

    private:
        uint64_t m_opt_delta;
        uint64_t m_opt_len;
        uint8_t m_opt_delta_1;
        bool n_opt_delta_1;

    public:
        bool _is_null_opt_delta_1() { opt_delta_1(); return n_opt_delta_1; };

    private:
        uint16_t m_opt_delta_2;
        bool n_opt_delta_2;

    public:
        bool _is_null_opt_delta_2() { opt_delta_2(); return n_opt_delta_2; };

    private:
        uint8_t m_opt_len_1;
        bool n_opt_len_1;

    public:
        bool _is_null_opt_len_1() { opt_len_1(); return n_opt_len_1; };

    private:
        uint16_t m_opt_len_2;
        bool n_opt_len_2;

    public:
        bool _is_null_opt_len_2() { opt_len_2(); return n_opt_len_2; };

    private:
        std::string m_value;
        coap_t* m__root;
        coap_t* m__parent;

    public:
        uint64_t opt_delta() const { return m_opt_delta; }
        uint64_t opt_len() const { return m_opt_len; }
        uint8_t opt_delta_1() const { return m_opt_delta_1; }
        uint16_t opt_delta_2() const { return m_opt_delta_2; }
        uint8_t opt_len_1() const { return m_opt_len_1; }
        uint16_t opt_len_2() const { return m_opt_len_2; }
        std::string value() const { return m_value; }
        coap_t* _root() const { return m__root; }
        coap_t* _parent() const { return m__parent; }
    };

private:
    uint64_t m_version;
    coap_message_type_t m_type;
    uint64_t m_tkl;
    coap_code_t m_code;
    uint16_t m_message_id;
    std::string m_token;
    std::vector<option_t*>* m_options;
    bool n_options;

public:
    bool _is_null_options() { options(); return n_options; };

private:
    std::string m_body;
    coap_t* m__root;
    kaitai::kstruct* m__parent;

public:
    uint64_t version() const { return m_version; }
    coap_message_type_t type() const { return m_type; }
    uint64_t tkl() const { return m_tkl; }
    coap_code_t code() const { return m_code; }
    uint16_t message_id() const { return m_message_id; }
    std::string token() const { return m_token; }
    std::vector<option_t*>* options() const { return m_options; }
    std::string body() const { return m_body; }
    coap_t* _root() const { return m__root; }
    kaitai::kstruct* _parent() const { return m__parent; }
};

#endif  // COAP_H_
