# This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

require 'kaitai/struct/struct'
require 'zlib'

unless Gem::Version.new(Kaitai::Struct::VERSION) >= Gem::Version.new('0.7')
  raise "Incompatible Kaitai Struct Ruby API: 0.7 or later is required, but you have #{Kaitai::Struct::VERSION}"
end

class Coap < Kaitai::Struct::Struct

  COAP_MESSAGE_TYPE = {
    0 => :coap_message_type_confirmable,
    1 => :coap_message_type_non_comfirmanble,
    2 => :coap_message_type_acknowledgement,
    3 => :coap_message_type_reset,
  }
  I__COAP_MESSAGE_TYPE = COAP_MESSAGE_TYPE.invert

  COAP_CODE = {
    0 => :coap_code_empty,
    1 => :coap_code_get,
    2 => :coap_code_post,
    3 => :coap_code_put,
    4 => :coap_code_delete,
    65 => :coap_code_created,
    66 => :coap_code_deleted,
    67 => :coap_code_valid,
    68 => :coap_code_changed,
    69 => :coap_code_content,
    128 => :coap_code_bad_request,
    129 => :coap_code_unathorized,
    130 => :coap_code_bad_option,
    131 => :coap_code_forbidden,
    132 => :coap_code_not_found,
    133 => :coap_code_method_not_allowed,
    134 => :coap_code_not_acceptable,
    140 => :coap_code_precondition_failed,
    141 => :coap_code_request_entity_too_large,
    143 => :coap_code_unsupported_content_format,
    160 => :coap_code_internal_server_error,
    161 => :coap_code_not_implemented,
    162 => :coap_code_bad_gateway,
    163 => :coap_code_service_unavailable,
    164 => :coap_code_gateway_timeout,
    165 => :coap_code_proxying_not_supported,
  }
  I__COAP_CODE = COAP_CODE.invert

  COAP_OPTIONS = {
    1 => :coap_options_if_match,
    3 => :coap_options_uri_host,
    4 => :coap_options_etag,
    5 => :coap_options_if_none_match,
    7 => :coap_options_uri_port,
    8 => :coap_options_location_path,
    11 => :coap_options_uri_path,
    12 => :coap_options_content_format,
    14 => :coap_options_max_age,
    15 => :coap_options_uri_query,
    17 => :coap_options_accept,
    20 => :coap_options_location_query,
    35 => :coap_options_proxy_uri,
    39 => :coap_options_proxy_scheme,
    60 => :coap_options_size1,
  }
  I__COAP_OPTIONS = COAP_OPTIONS.invert
  def initialize(_io, _parent = nil, _root = self)
    super(_io, _parent, _root)
    @version = @_io.read_bits_int(2)
    @type = Kaitai::Struct::Stream::resolve_enum(COAP_MESSAGE_TYPE, @_io.read_bits_int(2))
    @tkl = @_io.read_bits_int(4)
    @_io.align_to_byte
    @code = Kaitai::Struct::Stream::resolve_enum(COAP_CODE, @_io.read_u1)
    @message_id = @_io.read_u2be
    @token = @_io.read_bytes(tkl)
    if _io.eof? == false
      @options = []
      begin
        _ = Option.new(@_io, self, @_root)
        @options << _
      end until  ((_.is_payload_marker) || (_io.eof?)) 
    end
    @body = @_io.read_bytes_full
  end

  ##
  # Each option instance in a message specifies the Option Number of the defined CoAP option, the length of the Option Value, and the Option Value itself. Option nunber is expressed as delta. Both option length and delta values uses packing. Option is represented as  4 bits for regular values from 0-12. Values 13 and 14 informs that  option length is provided in extra bytes. The same holds for delta. 
  class Option < Kaitai::Struct::Struct
    def initialize(_io, _parent = nil, _root = self)
      super(_io, _parent, _root)
      @opt_delta = @_io.read_bits_int(4)
      @opt_len = @_io.read_bits_int(4)
      @_io.align_to_byte
      if opt_delta == 13
        @opt_delta_1 = @_io.read_u1
      end
      if opt_delta == 14
        @opt_delta_2 = @_io.read_u2be
      end
      if opt_len == 13
        @opt_len_1 = @_io.read_u1
      end
      if opt_len == 14
        @opt_len_2 = @_io.read_u2be
      end
      @value = @_io.read_bytes(length)
    end
    def length
      return @length unless @length.nil?
      @length = (opt_len == 13 ? opt_len_1 : (opt_len == 14 ? opt_len_2 : (opt_len == 15 ? 0 : opt_len)))
      @length
    end
    def delta
      return @delta unless @delta.nil?
      @delta = (opt_delta == 13 ? opt_delta_1 : (opt_delta == 14 ? opt_delta_2 : (opt_delta == 15 ? 0 : opt_delta)))
      @delta
    end
    def is_payload_marker
      return @is_payload_marker unless @is_payload_marker.nil?
      @is_payload_marker =  ((opt_len == 15) && (opt_delta == 15)) 
      @is_payload_marker
    end
    attr_reader :opt_delta
    attr_reader :opt_len
    attr_reader :opt_delta_1
    attr_reader :opt_delta_2
    attr_reader :opt_len_1
    attr_reader :opt_len_2
    attr_reader :value
  end
  attr_reader :version
  attr_reader :type
  attr_reader :tkl
  attr_reader :code
  attr_reader :message_id
  attr_reader :token
  attr_reader :options
  attr_reader :body
end
