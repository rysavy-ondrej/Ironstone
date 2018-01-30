# This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

require 'kaitai/struct/struct'
require 'zlib'

unless Gem::Version.new(Kaitai::Struct::VERSION) >= Gem::Version.new('0.7')
  raise "Incompatible Kaitai Struct Ruby API: 0.7 or later is required, but you have #{Kaitai::Struct::VERSION}"
end


##
# The binary TLV (Type-Length-Value) format is used to represent an array of values  or a singular value using a compact binary representation, which is easy to process  on simple embedded devices. The format has a minimum overhead per value of just 2 bytes  and a maximum overhead of 5 bytes depending on the type of Identifier and length of the value.  The maximum size of an Object Instance or Resource in this format is 16.7 MB.  The format is self- describing, thus a parser can skip TLVs for which the Resource is not known. This data format has a Media Type of application/vnd.oma.lwm2m+tlv.
class Lwm2mTlv < Kaitai::Struct::Struct

  LWM2M_TLV_IDENTIFIER_TYPE = {
    0 => :lwm2m_tlv_identifier_type_object_instance,
    1 => :lwm2m_tlv_identifier_type_resource_instance,
    2 => :lwm2m_tlv_identifier_type_multiple_resource,
    3 => :lwm2m_tlv_identifier_type_resource_with_value,
  }
  I__LWM2M_TLV_IDENTIFIER_TYPE = LWM2M_TLV_IDENTIFIER_TYPE.invert
  def initialize(_io, _parent = nil, _root = self)
    super(_io, _parent, _root)
    @type = TlvType.new(@_io, self, @_root)
    @identifier = TlvIdentifier.new(@_io, self, @_root)
    @length = TlvLength.new(@_io, self, @_root)
    @value = @_io.read_bytes(length.value)
  end
  class TlvIdentifier < Kaitai::Struct::Struct
    def initialize(_io, _parent = nil, _root = self)
      super(_io, _parent, _root)
      if _parent.type.identifier_wide_length == false
        @tlv_id_1 = @_io.read_u1
      end
      if _parent.type.identifier_wide_length == true
        @tlv_id_2 = @_io.read_u2be
      end
    end
    def value
      return @value unless @value.nil?
      @value = (tlv_id_1 | tlv_id_2)
      @value
    end
    attr_reader :tlv_id_1
    attr_reader :tlv_id_2
  end
  class TlvLength < Kaitai::Struct::Struct
    def initialize(_io, _parent = nil, _root = self)
      super(_io, _parent, _root)
      if _parent.type.length_type == 1
        @tlv_len_1 = @_io.read_u1
      end
      if _parent.type.length_type == 2
        @tlv_len_2 = @_io.read_bits_int(16)
      end
      if _parent.type.length_type == 3
        @tlv_len_3 = @_io.read_bits_int(24)
      end
    end
    def value
      return @value unless @value.nil?
      @value = (((_parent.type.value_length | tlv_len_1) | tlv_len_2) | tlv_len_3)
      @value
    end
    attr_reader :tlv_len_1
    attr_reader :tlv_len_2
    attr_reader :tlv_len_3
  end
  class TlvType < Kaitai::Struct::Struct
    def initialize(_io, _parent = nil, _root = self)
      super(_io, _parent, _root)
      @identifier_type = Kaitai::Struct::Stream::resolve_enum(LWM2M_TLV_IDENTIFIER_TYPE, @_io.read_bits_int(2))
      @identifier_wide_length = @_io.read_bits_int(1) != 0
      @length_type = @_io.read_bits_int(2)
      @value_length = @_io.read_bits_int(3)
    end

    ##
    # Bits 7-6: Indicates the type of Identifier:
    #   00= Object Instance in which case the Value contains one or more Resource TLVs
    #   01= Resource Instance with Value for use within a multiple Resource TLV
    #   10= multiple Resource, in which case the Value contains one or more Resource Instance TLVs
    #   11= Resource with Value
    attr_reader :identifier_type

    ##
    # Bit 5: Indicates the Length of the Identifier. 
    #   0=The Identifier field of this TLV is 8 bits long
    #   1=The Identifier field of this TLV is 16 bits long
    attr_reader :identifier_wide_length

    ##
    # Bit 4-3: Indicates the type of Length.
    #   00 = No length field, the value immediately follows the Identifier field in is of the length indicated by Bits 2-0 of this field
    #   01 = The Length field is 8-bits and Bits 2-0 MUST be ignored
    #   10 = The Length field is 16-bits and Bits 2-0 MUST be ignored
    #   11 = The Length field is 24-bits and Bits 2-0 MUST be ignored
    attr_reader :length_type

    ##
    # Bits 2-0: A 3-bit unsigned integer indicating the Length of the Value.
    attr_reader :value_length
  end

  ##
  # 8-bits masked field
  attr_reader :type

  ##
  # The Object Instance, Resource, or Resource Instance ID as indicated by the Type field.
  attr_reader :identifier

  ##
  # The Length of the following field in bytes.
  attr_reader :length

  ##
  # Value of the tag. The format of the value depends on the Resource‟s data type.
  attr_reader :value
end
