# using tshark to export information about coap traffic:
# note that tshark should be at least version 
tshark -T fields -e frame.time_epoch -e ip.src -e ip.dst -e udp.srcport -e udp.dstport -e udp.length -e coap.code -e coap.type -e coap.mid -e coap.token -e coap.opt.uri_path_recon -E header=y -E separator=, -r $1 > $1.csv
