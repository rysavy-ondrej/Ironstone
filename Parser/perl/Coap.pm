# This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

use strict;
use warnings;
use IO::KaitaiStruct 0.007_000;
use Compress::Zlib;
use Encode;

########################################################################
package Coap;

our @ISA = 'IO::KaitaiStruct::Struct';

sub from_file {
    my ($class, $filename) = @_;
    my $fd;

    open($fd, '<', $filename) or return undef;
    binmode($fd);
    return new($class, IO::KaitaiStruct::Stream->new($fd));
}

our $COAP_MESSAGE_TYPE_CONFIRMABLE = 0;
our $COAP_MESSAGE_TYPE_NON_COMFIRMANBLE = 1;
our $COAP_MESSAGE_TYPE_ACKNOWLEDGEMENT = 2;
our $COAP_MESSAGE_TYPE_RESET = 3;

our $COAP_CODE_EMPTY = 0;
our $COAP_CODE_GET = 1;
our $COAP_CODE_POST = 2;
our $COAP_CODE_PUT = 3;
our $COAP_CODE_DELETE = 4;
our $COAP_CODE_CREATED = 65;
our $COAP_CODE_DELETED = 66;
our $COAP_CODE_VALID = 67;
our $COAP_CODE_CHANGED = 68;
our $COAP_CODE_CONTENT = 69;
our $COAP_CODE_BAD_REQUEST = 128;
our $COAP_CODE_UNATHORIZED = 129;
our $COAP_CODE_BAD_OPTION = 130;
our $COAP_CODE_FORBIDDEN = 131;
our $COAP_CODE_NOT_FOUND = 132;
our $COAP_CODE_METHOD_NOT_ALLOWED = 133;
our $COAP_CODE_NOT_ACCEPTABLE = 134;
our $COAP_CODE_PRECONDITION_FAILED = 140;
our $COAP_CODE_REQUEST_ENTITY_TOO_LARGE = 141;
our $COAP_CODE_UNSUPPORTED_CONTENT_FORMAT = 143;
our $COAP_CODE_INTERNAL_SERVER_ERROR = 160;
our $COAP_CODE_NOT_IMPLEMENTED = 161;
our $COAP_CODE_BAD_GATEWAY = 162;
our $COAP_CODE_SERVICE_UNAVAILABLE = 163;
our $COAP_CODE_GATEWAY_TIMEOUT = 164;
our $COAP_CODE_PROXYING_NOT_SUPPORTED = 165;

our $COAP_OPTIONS_IF_MATCH = 1;
our $COAP_OPTIONS_URI_HOST = 3;
our $COAP_OPTIONS_ETAG = 4;
our $COAP_OPTIONS_IF_NONE_MATCH = 5;
our $COAP_OPTIONS_URI_PORT = 7;
our $COAP_OPTIONS_LOCATION_PATH = 8;
our $COAP_OPTIONS_URI_PATH = 11;
our $COAP_OPTIONS_CONTENT_FORMAT = 12;
our $COAP_OPTIONS_MAX_AGE = 14;
our $COAP_OPTIONS_URI_QUERY = 15;
our $COAP_OPTIONS_ACCEPT = 17;
our $COAP_OPTIONS_LOCATION_QUERY = 20;
our $COAP_OPTIONS_PROXY_URI = 35;
our $COAP_OPTIONS_PROXY_SCHEME = 39;
our $COAP_OPTIONS_SIZE1 = 60;

sub new {
    my ($class, $_io, $_parent, $_root) = @_;
    my $self = IO::KaitaiStruct::Struct->new($_io);

    bless $self, $class;
    $self->{_parent} = $_parent;
    $self->{_root} = $_root || $self;

    $self->{version} = $self->{_io}->read_bits_int(2);
    $self->{type} = $self->{_io}->read_bits_int(2);
    $self->{tkl} = $self->{_io}->read_bits_int(4);
    $self->{_io}->align_to_byte();
    $self->{code} = $self->{_io}->read_u1();
    $self->{message_id} = $self->{_io}->read_u2be();
    $self->{token} = $self->{_io}->read_bytes($self->tkl());
    if ($self->_io()->is_eof() == 0) {
        $self->{options} = ();
        do {
            $_ = Coap::Option->new($self->{_io}, $self, $self->{_root});
            push @{$self->{options}}, $_;
        } until ( (($_->is_payload_marker()) || ($self->_io()->is_eof())) );
    }
    $self->{body} = $self->{_io}->read_bytes_full();

    return $self;
}

sub version {
    my ($self) = @_;
    return $self->{version};
}

sub type {
    my ($self) = @_;
    return $self->{type};
}

sub tkl {
    my ($self) = @_;
    return $self->{tkl};
}

sub code {
    my ($self) = @_;
    return $self->{code};
}

sub message_id {
    my ($self) = @_;
    return $self->{message_id};
}

sub token {
    my ($self) = @_;
    return $self->{token};
}

sub options {
    my ($self) = @_;
    return $self->{options};
}

sub body {
    my ($self) = @_;
    return $self->{body};
}

########################################################################
package Coap::Option;

our @ISA = 'IO::KaitaiStruct::Struct';

sub from_file {
    my ($class, $filename) = @_;
    my $fd;

    open($fd, '<', $filename) or return undef;
    binmode($fd);
    return new($class, IO::KaitaiStruct::Stream->new($fd));
}

sub new {
    my ($class, $_io, $_parent, $_root) = @_;
    my $self = IO::KaitaiStruct::Struct->new($_io);

    bless $self, $class;
    $self->{_parent} = $_parent;
    $self->{_root} = $_root || $self;

    $self->{opt_delta} = $self->{_io}->read_bits_int(4);
    $self->{opt_len} = $self->{_io}->read_bits_int(4);
    $self->{_io}->align_to_byte();
    if ($self->opt_delta() == 13) {
        $self->{opt_delta_1} = $self->{_io}->read_u1();
    }
    if ($self->opt_delta() == 14) {
        $self->{opt_delta_2} = $self->{_io}->read_u2be();
    }
    if ($self->opt_len() == 13) {
        $self->{opt_len_1} = $self->{_io}->read_u1();
    }
    if ($self->opt_len() == 14) {
        $self->{opt_len_2} = $self->{_io}->read_u2be();
    }
    $self->{value} = $self->{_io}->read_bytes($self->length());

    return $self;
}

sub length {
    my ($self) = @_;
    return $self->{length} if ($self->{length});
    $self->{length} = ($self->opt_len() == 13 ? $self->opt_len_1() : ($self->opt_len() == 14 ? $self->opt_len_2() : ($self->opt_len() == 15 ? 0 : $self->opt_len())));
    return $self->{length};
}

sub delta {
    my ($self) = @_;
    return $self->{delta} if ($self->{delta});
    $self->{delta} = ($self->opt_delta() == 13 ? $self->opt_delta_1() : ($self->opt_delta() == 14 ? $self->opt_delta_2() : ($self->opt_delta() == 15 ? 0 : $self->opt_delta())));
    return $self->{delta};
}

sub is_payload_marker {
    my ($self) = @_;
    return $self->{is_payload_marker} if ($self->{is_payload_marker});
    $self->{is_payload_marker} =  (($self->opt_len() == 15) && ($self->opt_delta() == 15)) ;
    return $self->{is_payload_marker};
}

sub opt_delta {
    my ($self) = @_;
    return $self->{opt_delta};
}

sub opt_len {
    my ($self) = @_;
    return $self->{opt_len};
}

sub opt_delta_1 {
    my ($self) = @_;
    return $self->{opt_delta_1};
}

sub opt_delta_2 {
    my ($self) = @_;
    return $self->{opt_delta_2};
}

sub opt_len_1 {
    my ($self) = @_;
    return $self->{opt_len_1};
}

sub opt_len_2 {
    my ($self) = @_;
    return $self->{opt_len_2};
}

sub value {
    my ($self) = @_;
    return $self->{value};
}

1;
