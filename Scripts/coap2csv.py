#!/usr/bin/python
# usage: coap2csv.py -i <inputfile> -o <outputfile>
import sys, getopt
import pyshark  # pip install pyshark

def print_csv(packet):
    
    if int(packet.coap.code) > 0:
        if int(packet.coap.code) < 32:
            uri_host = '{}:{}'.format(packet.ip.dst,packet.udp.dstport)
        else:
            uri_host = '{}:{}'.format(packet.ip.src,packet.udp.srcport)
    else:
        uri_host = ''
    
    # print(packet.coap.field_names)

    try:
        uri_path = packet.coap.opt_uri_path
    except AttributeError as e: 
        uri_path = 'x'

    print('{},{},{},{},{},{},{},{},{},{}'.format(
            packet.sniff_timestamp,
            packet.ip.src,
            packet.udp.srcport,
            packet.ip.dst,
            packet.udp.dstport,
            packet.udp.length,
            packet.coap.code,
            packet.coap.type,
            uri_host, 
            uri_path))


def main(argv):
    inputfile = ''
    outputfile = ''
    try:
        opts, args = getopt.getopt(argv,"hi:o:",["ifile=","ofile="])
    except getopt.GetoptError:
        print('Usage: test.py -i <inputfile> -o <outputfile>')
        sys.exit(2)
   
    for opt, arg in opts:
        if opt == '-h':
            print('Usage: test.py -i <inputfile> -o <outputfile>')
            sys.exit()
        elif opt in ("-i", "--ifile"):
            inputfile = arg
        elif opt in ("-o", "--ofile"):
            outputfile = arg

    if inputfile == '':
        print('Error: Input file not specified!')
        print('Usage: test.py -i <inputfile> -o <outputfile>')
        sys.exit(2)

    capture = pyshark.FileCapture(inputfile)
    print('Start Msec, L3 Ipv4 Src, L4 Port Src, L3 Ipv4 Dst, L4 Port Dst, L4 Paylen, Coap Code, Coap Type, Coap Uri Host, Coap Uri Path')
    for packet in capture:
        print_csv(packet)


if __name__ == "__main__":
   main(sys.argv[1:])
