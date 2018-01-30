// This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

#include "lwm2m_tlv.h"

#include <iostream>
#include <fstream>

lwm2m_tlv_t::lwm2m_tlv_t(kaitai::kstream *p_io, kaitai::kstruct *p_parent, lwm2m_tlv_t *p_root) : kaitai::kstruct(p_io) {
    m__parent = p_parent;
    m__root = this;
    m_type = new tlv_type_t(m__io, this, m__root);
    m_identifier = new tlv_identifier_t(m__io, this, m__root);
    m_length = new tlv_length_t(m__io, this, m__root);
    m_value = m__io->read_bytes(length()->value());
}

lwm2m_tlv_t::~lwm2m_tlv_t() {
    delete m_type;
    delete m_identifier;
    delete m_length;
}

lwm2m_tlv_t::tlv_identifier_t::tlv_identifier_t(kaitai::kstream *p_io, lwm2m_tlv_t *p_parent, lwm2m_tlv_t *p_root) : kaitai::kstruct(p_io) {
    m__parent = p_parent;
    m__root = p_root;
    f_value = false;
    n_tlv_id_1 = true;
    if (_parent()->type()->identifier_wide_length() == false) {
        n_tlv_id_1 = false;
        m_tlv_id_1 = m__io->read_u1();
    }
    n_tlv_id_2 = true;
    if (_parent()->type()->identifier_wide_length() == true) {
        n_tlv_id_2 = false;
        m_tlv_id_2 = m__io->read_u2be();
    }
}

lwm2m_tlv_t::tlv_identifier_t::~tlv_identifier_t() {
}

int32_t lwm2m_tlv_t::tlv_identifier_t::value() {
    if (f_value)
        return m_value;
    m_value = (tlv_id_1() | tlv_id_2());
    f_value = true;
    return m_value;
}

lwm2m_tlv_t::tlv_length_t::tlv_length_t(kaitai::kstream *p_io, lwm2m_tlv_t *p_parent, lwm2m_tlv_t *p_root) : kaitai::kstruct(p_io) {
    m__parent = p_parent;
    m__root = p_root;
    f_value = false;
    n_tlv_len_1 = true;
    if (_parent()->type()->length_type() == 1) {
        n_tlv_len_1 = false;
        m_tlv_len_1 = m__io->read_u1();
    }
    n_tlv_len_2 = true;
    if (_parent()->type()->length_type() == 2) {
        n_tlv_len_2 = false;
        m_tlv_len_2 = m__io->read_bits_int(16);
    }
    n_tlv_len_3 = true;
    if (_parent()->type()->length_type() == 3) {
        n_tlv_len_3 = false;
        m_tlv_len_3 = m__io->read_bits_int(24);
    }
}

lwm2m_tlv_t::tlv_length_t::~tlv_length_t() {
}

int32_t lwm2m_tlv_t::tlv_length_t::value() {
    if (f_value)
        return m_value;
    m_value = (((_parent()->type()->value_length() | tlv_len_1()) | tlv_len_2()) | tlv_len_3());
    f_value = true;
    return m_value;
}

lwm2m_tlv_t::tlv_type_t::tlv_type_t(kaitai::kstream *p_io, lwm2m_tlv_t *p_parent, lwm2m_tlv_t *p_root) : kaitai::kstruct(p_io) {
    m__parent = p_parent;
    m__root = p_root;
    m_identifier_type = static_cast<lwm2m_tlv_t::lwm2m_tlv_identifier_type_t>(m__io->read_bits_int(2));
    m_identifier_wide_length = m__io->read_bits_int(1);
    m_length_type = m__io->read_bits_int(2);
    m_value_length = m__io->read_bits_int(3);
}

lwm2m_tlv_t::tlv_type_t::~tlv_type_t() {
}
