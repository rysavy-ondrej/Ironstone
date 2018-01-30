# This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

import array
import struct
import zlib
from enum import Enum
from pkg_resources import parse_version

from kaitaistruct import __version__ as ks_version, KaitaiStruct, KaitaiStream, BytesIO


if parse_version(ks_version) < parse_version('0.7'):
    raise Exception("Incompatible Kaitai Struct Python API: 0.7 or later is required, but you have %s" % (ks_version))

class Lwm2mTlv(KaitaiStruct):

    class Lwm2mTlvIdentifierType(Enum):
        object_instance = 0
        resource_instance = 1
        multiple_resource = 2
        resource_with_value = 3
    def __init__(self, _io, _parent=None, _root=None):
        self._io = _io
        self._parent = _parent
        self._root = _root if _root else self
        self.type = self._root.TlvType(self._io, self, self._root)
        self.identifier = self._root.TlvIdentifier(self._io, self, self._root)
        self.length = self._root.TlvLength(self._io, self, self._root)
        self.value = self._io.read_bytes(self.length.value)

    class TlvIdentifier(KaitaiStruct):
        def __init__(self, _io, _parent=None, _root=None):
            self._io = _io
            self._parent = _parent
            self._root = _root if _root else self
            if self._parent.type.identifier_wide_length == False:
                self.tlv_id_1 = self._io.read_u1()

            if self._parent.type.identifier_wide_length == True:
                self.tlv_id_2 = self._io.read_u2be()


        @property
        def value(self):
            if hasattr(self, '_m_value'):
                return self._m_value if hasattr(self, '_m_value') else None

            self._m_value = (self.tlv_id_1 | self.tlv_id_2)
            return self._m_value if hasattr(self, '_m_value') else None


    class TlvLength(KaitaiStruct):
        def __init__(self, _io, _parent=None, _root=None):
            self._io = _io
            self._parent = _parent
            self._root = _root if _root else self
            if self._parent.type.length_type == 1:
                self.tlv_len_1 = self._io.read_u1()

            if self._parent.type.length_type == 2:
                self.tlv_len_2 = self._io.read_bits_int(16)

            if self._parent.type.length_type == 3:
                self.tlv_len_3 = self._io.read_bits_int(24)


        @property
        def value(self):
            if hasattr(self, '_m_value'):
                return self._m_value if hasattr(self, '_m_value') else None

            self._m_value = (((self._parent.type.value_length | self.tlv_len_1) | self.tlv_len_2) | self.tlv_len_3)
            return self._m_value if hasattr(self, '_m_value') else None


    class TlvType(KaitaiStruct):
        def __init__(self, _io, _parent=None, _root=None):
            self._io = _io
            self._parent = _parent
            self._root = _root if _root else self
            self.identifier_type = self._root.Lwm2mTlvIdentifierType(self._io.read_bits_int(2))
            self.identifier_wide_length = self._io.read_bits_int(1) != 0
            self.length_type = self._io.read_bits_int(2)
            self.value_length = self._io.read_bits_int(3)



