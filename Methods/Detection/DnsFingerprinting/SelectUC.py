
# Created in July 2019 by Ivana Burgetova
# Project: DNS Profiling



"""
This script selects unicast DNS queries from the input file.
The script expects the following input file structure:
each line constains one DNS query, individual values are separated by semicolon:
srcIP;serverIP;query-type;domain-name

This script can be used for selecting unicast queries from provided files Dataset1.txt and Dataset2.txt.

The script also rearranges the individual values 
and print them into standard output in the following order:
srcIP;domain-name;query-type;serverIP
"""


from argparse import ArgumentParser

#definition of the arguments
parser = ArgumentParser(description='The argument -f is required to specify the input file.')  
parser.add_argument("-f","--input_file", required=True, help='the input file with DNS queries')

args = parser.parse_args()

input_file_name = args.input_file

i_file = open(input_file_name, "r")
for line in i_file:
   line2 = line.strip()
   fields = line2.split(";")  
   if fields[1] != "224.0.0.251" and fields[1] != "224.0.0.252":
      s = fields[0] + ";" + fields[3] + ";" + fields[2] + ";" + fields[1]
      print (s)    
i_file.close()

