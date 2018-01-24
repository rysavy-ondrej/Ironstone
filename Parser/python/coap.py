# This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

import array
import struct
import zlib
from enum import Enum
from pkg_resources import parse_version

from kaitaistruct import __version__ as ks_version, KaitaiStruct, KaitaiStream, BytesIO


if parse_version(ks_version) < parse_version('0.7'):
    raise Exception("Incompatible Kaitai Struct Python API: 0.7 or later is required, but you have %s" % (ks_version))

class Coap(KaitaiStruct):

    class CoapMessageType(Enum):
        confirmable = 0
        non_comfirmanble = 1
        acknowledgement = 2
        reset = 3

    class CoapCode(Enum):
        empty = 0
        get = 1
        post = 2
        put = 3
        delete = 4
        created = 65
        deleted = 66
        valid = 67
        changed = 68
        content = 69
        bad_request = 128
        unathorized = 129
        bad_option = 130
        forbidden = 131
        not_found = 132
        method_not_allowed = 133
        not_acceptable = 134
        precondition_failed = 140
        request_entity_too_large = 141
        unsupported_content_format = 143
        internal_server_error = 160
        not_implemented = 161
        bad_gateway = 162
        service_unavailable = 163
        gateway_timeout = 164
        proxying_not_supported = 165

    class CoapOptions(Enum):
        if_match = 1
        uri_host = 3
        etag = 4
        if_none_match = 5
        uri_port = 7
        location_path = 8
        uri_path = 11
        content_format = 12
        max_age = 14
        uri_query = 15
        accept = 17
        location_query = 20
        proxy_uri = 35
        proxy_scheme = 39
        size1 = 60
    def __init__(self, _io, _parent=None, _root=None):
        self._io = _io
        self._parent = _parent
        self._root = _root if _root else self
        self.version = self._io.read_bits_int(2)
        self.type = self._root.CoapMessageType(self._io.read_bits_int(2))
        self.tkl = self._io.read_bits_int(4)
        self._io.align_to_byte()
        self.code = self._root.CoapCode(self._io.read_u1())
        self.message_id = self._io.read_u2be()
        self.token = self._io.read_bytes(self.tkl)
        if self._io.is_eof() == False:
            self.options = []
            while True:
                _ = self._root.Option(self._io, self, self._root)
                self.options.append(_)
                if  ((_.is_payload_marker) or (self._io.is_eof())) :
                    break

        self.body = self._io.read_bytes_full()

    class Option(KaitaiStruct):
        def __init__(self, _io, _parent=None, _root=None):
            self._io = _io
            self._parent = _parent
            self._root = _root if _root else self
            self.opt_delta = self._io.read_bits_int(4)
            self.opt_len = self._io.read_bits_int(4)
            self._io.align_to_byte()
            if self.opt_delta == 13:
                self.opt_delta_1 = self._io.read_u1()

            if self.opt_delta == 14:
                self.opt_delta_2 = self._io.read_u2be()

            if self.opt_len == 13:
                self.opt_len_1 = self._io.read_u1()

            if self.opt_len == 14:
                self.opt_len_2 = self._io.read_u2be()

            self.value = self._io.read_bytes(self.length)

        @property
        def length(self):
            if hasattr(self, '_m_length'):
                return self._m_length if hasattr(self, '_m_length') else None

            self._m_length = (self.opt_len_1 if self.opt_len == 13 else (self.opt_len_2 if self.opt_len == 14 else (0 if self.opt_len == 15 else self.opt_len)))
            return self._m_length if hasattr(self, '_m_length') else None

        @property
        def delta(self):
            if hasattr(self, '_m_delta'):
                return self._m_delta if hasattr(self, '_m_delta') else None

            self._m_delta = (self.opt_delta_1 if self.opt_delta == 13 else (self.opt_delta_2 if self.opt_delta == 14 else (0 if self.opt_delta == 15 else self.opt_delta)))
            return self._m_delta if hasattr(self, '_m_delta') else None

        @property
        def is_payload_marker(self):
            if hasattr(self, '_m_is_payload_marker'):
                return self._m_is_payload_marker if hasattr(self, '_m_is_payload_marker') else None

            self._m_is_payload_marker =  ((self.opt_len == 15) and (self.opt_delta == 15)) 
            return self._m_is_payload_marker if hasattr(self, '_m_is_payload_marker') else None



