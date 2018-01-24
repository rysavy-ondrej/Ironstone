// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

#include "coap.h"

#include <iostream>
#include <fstream>

coap_t::coap_t(kaitai::kstream *p_io, kaitai::kstruct *p_parent, coap_t *p_root) : kaitai::kstruct(p_io) {
    m__parent = p_parent;
    m__root = this;
    m_version = m__io->read_bits_int(2);
    m_type = static_cast<coap_t::coap_message_type_t>(m__io->read_bits_int(2));
    m_tkl = m__io->read_bits_int(4);
    m__io->align_to_byte();
    m_code = static_cast<coap_t::coap_code_t>(m__io->read_u1());
    m_message_id = m__io->read_u2be();
    m_token = m__io->read_bytes(tkl());
    n_options = true;
    if (_io()->is_eof() == false) {
        n_options = false;
        m_options = new std::vector<option_t*>();
        {
            option_t* _;
            do {
                _ = new option_t(m__io, this, m__root);
                m_options->push_back(_);
            } while (!( ((_->is_payload_marker()) || (_io()->is_eof())) ));
        }
    }
    m_body = m__io->read_bytes_full();
}

coap_t::~coap_t() {
    if (!n_options) {
        for (std::vector<option_t*>::iterator it = m_options->begin(); it != m_options->end(); ++it) {
            delete *it;
        }
        delete m_options;
    }
}

coap_t::option_t::option_t(kaitai::kstream *p_io, coap_t *p_parent, coap_t *p_root) : kaitai::kstruct(p_io) {
    m__parent = p_parent;
    m__root = p_root;
    f_length = false;
    f_delta = false;
    f_is_payload_marker = false;
    m_opt_delta = m__io->read_bits_int(4);
    m_opt_len = m__io->read_bits_int(4);
    m__io->align_to_byte();
    n_opt_delta_1 = true;
    if (opt_delta() == 13) {
        n_opt_delta_1 = false;
        m_opt_delta_1 = m__io->read_u1();
    }
    n_opt_delta_2 = true;
    if (opt_delta() == 14) {
        n_opt_delta_2 = false;
        m_opt_delta_2 = m__io->read_u2be();
    }
    n_opt_len_1 = true;
    if (opt_len() == 13) {
        n_opt_len_1 = false;
        m_opt_len_1 = m__io->read_u1();
    }
    n_opt_len_2 = true;
    if (opt_len() == 14) {
        n_opt_len_2 = false;
        m_opt_len_2 = m__io->read_u2be();
    }
    m_value = m__io->read_bytes(length());
}

coap_t::option_t::~option_t() {
}

int32_t coap_t::option_t::length() {
    if (f_length)
        return m_length;
    m_length = ((opt_len() == 13) ? (opt_len_1()) : (((opt_len() == 14) ? (opt_len_2()) : (((opt_len() == 15) ? (0) : (opt_len()))))));
    f_length = true;
    return m_length;
}

int32_t coap_t::option_t::delta() {
    if (f_delta)
        return m_delta;
    m_delta = ((opt_delta() == 13) ? (opt_delta_1()) : (((opt_delta() == 14) ? (opt_delta_2()) : (((opt_delta() == 15) ? (0) : (opt_delta()))))));
    f_delta = true;
    return m_delta;
}

bool coap_t::option_t::is_payload_marker() {
    if (f_is_payload_marker)
        return m_is_payload_marker;
    m_is_payload_marker =  ((opt_len() == 15) && (opt_delta() == 15)) ;
    f_is_payload_marker = true;
    return m_is_payload_marker;
}
