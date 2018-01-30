# This is a generated file! Please edit source .ksy file and use kaitai-struct-compiler to rebuild

use strict;
use warnings;
use IO::KaitaiStruct 0.007_000;
use Compress::Zlib;
use Encode;

########################################################################
package Lwm2mTlv;

our @ISA = 'IO::KaitaiStruct::Struct';

sub from_file {
    my ($class, $filename) = @_;
    my $fd;

    open($fd, '<', $filename) or return undef;
    binmode($fd);
    return new($class, IO::KaitaiStruct::Stream->new($fd));
}

our $LWM2M_TLV_IDENTIFIER_TYPE_OBJECT_INSTANCE = 0;
our $LWM2M_TLV_IDENTIFIER_TYPE_RESOURCE_INSTANCE = 1;
our $LWM2M_TLV_IDENTIFIER_TYPE_MULTIPLE_RESOURCE = 2;
our $LWM2M_TLV_IDENTIFIER_TYPE_RESOURCE_WITH_VALUE = 3;

sub new {
    my ($class, $_io, $_parent, $_root) = @_;
    my $self = IO::KaitaiStruct::Struct->new($_io);

    bless $self, $class;
    $self->{_parent} = $_parent;
    $self->{_root} = $_root || $self;

    $self->{type} = Lwm2mTlv::TlvType->new($self->{_io}, $self, $self->{_root});
    $self->{identifier} = Lwm2mTlv::TlvIdentifier->new($self->{_io}, $self, $self->{_root});
    $self->{length} = Lwm2mTlv::TlvLength->new($self->{_io}, $self, $self->{_root});
    $self->{value} = $self->{_io}->read_bytes($self->length()->value());

    return $self;
}

sub type {
    my ($self) = @_;
    return $self->{type};
}

sub identifier {
    my ($self) = @_;
    return $self->{identifier};
}

sub length {
    my ($self) = @_;
    return $self->{length};
}

sub value {
    my ($self) = @_;
    return $self->{value};
}

########################################################################
package Lwm2mTlv::TlvIdentifier;

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

    if ($self->_parent()->type()->identifier_wide_length() == 0) {
        $self->{tlv_id_1} = $self->{_io}->read_u1();
    }
    if ($self->_parent()->type()->identifier_wide_length() == 1) {
        $self->{tlv_id_2} = $self->{_io}->read_u2be();
    }

    return $self;
}

sub value {
    my ($self) = @_;
    return $self->{value} if ($self->{value});
    $self->{value} = ($self->tlv_id_1() | $self->tlv_id_2());
    return $self->{value};
}

sub tlv_id_1 {
    my ($self) = @_;
    return $self->{tlv_id_1};
}

sub tlv_id_2 {
    my ($self) = @_;
    return $self->{tlv_id_2};
}

########################################################################
package Lwm2mTlv::TlvLength;

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

    if ($self->_parent()->type()->length_type() == 1) {
        $self->{tlv_len_1} = $self->{_io}->read_u1();
    }
    if ($self->_parent()->type()->length_type() == 2) {
        $self->{tlv_len_2} = $self->{_io}->read_bits_int(16);
    }
    if ($self->_parent()->type()->length_type() == 3) {
        $self->{tlv_len_3} = $self->{_io}->read_bits_int(24);
    }

    return $self;
}

sub value {
    my ($self) = @_;
    return $self->{value} if ($self->{value});
    $self->{value} = ((($self->_parent()->type()->value_length() | $self->tlv_len_1()) | $self->tlv_len_2()) | $self->tlv_len_3());
    return $self->{value};
}

sub tlv_len_1 {
    my ($self) = @_;
    return $self->{tlv_len_1};
}

sub tlv_len_2 {
    my ($self) = @_;
    return $self->{tlv_len_2};
}

sub tlv_len_3 {
    my ($self) = @_;
    return $self->{tlv_len_3};
}

########################################################################
package Lwm2mTlv::TlvType;

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

    $self->{identifier_type} = $self->{_io}->read_bits_int(2);
    $self->{identifier_wide_length} = $self->{_io}->read_bits_int(1);
    $self->{length_type} = $self->{_io}->read_bits_int(2);
    $self->{value_length} = $self->{_io}->read_bits_int(3);

    return $self;
}

sub identifier_type {
    my ($self) = @_;
    return $self->{identifier_type};
}

sub identifier_wide_length {
    my ($self) = @_;
    return $self->{identifier_wide_length};
}

sub length_type {
    my ($self) = @_;
    return $self->{length_type};
}

sub value_length {
    my ($self) = @_;
    return $self->{value_length};
}

1;
