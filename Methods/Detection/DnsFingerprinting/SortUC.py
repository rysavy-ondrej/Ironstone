#!/usr/bin/env python
# -*- coding: utf-8 -*-

# Created in July 2019 by Ivana Burgetova
# Project: DNS Profiling

"""
This script sort unicast DNS queries from the input file.
The script expects the following input file structure:
each line constains one DNS query, individual values are separated by semicolon:
srcIP;domain-name;query-type;serverIP

Sorted DNS queries are printed to the standard output.
In this output the same queries from the given mobile device are on consecutive lines.

This script can be run on the output of the script selectUC.py.

"""


from argparse import ArgumentParser

#definition of the arguments
parser = ArgumentParser(description='The argument -f is required to specify the input file.')  
parser.add_argument("-f","--input_file", required=True, help='the input file with unicast DNS queries')

args = parser.parse_args()

input_file_name = args.input_file


with open(input_file_name) as f:
   content = f.readlines()  
f.close()

content.sort()
for x in content:
   x1 = x.rstrip('\n')
   print (x1)
print()