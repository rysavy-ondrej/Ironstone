
# Created in July 2019 by Ivana Burgetova
# Project: DNS Profiling

"""
This script parses the input file with sorted DNS queries 
and creates the corresponding SQL queries.
SQL queries are printed to the standard output.

The script expects the following input file structure:
each line constains one DNS query, individual values are separated by semicolon:
srcIP;domain-name;query-type;serverIP
and the same queries from the given mobile device are on consecutive lines.

"""


import mysql.connector
from argparse import ArgumentParser

#definition of the arguments
parser = ArgumentParser(description='The arguments allow to specify the details for database connection, the number of dataset and the input file.')  
parser.add_argument("-i","--input_host", required=True, help='server name or IP address on which MySQL is running')
parser.add_argument("-u","--input_user", required=True, help='the username that is used to work with MySQL Server')
parser.add_argument("-p","--input_passwd", required=True, help='the password that is used to work with MySQL Server')
parser.add_argument("-d","--input_database", required=True, help='database name to which you want to connect')
parser.add_argument("-n","--dataset", type=int, default = 1, help='number of dataset, specify the dataset for SQL queries')
parser.add_argument("-f","--input_file", required=True, help='the input file with sorted DNS queries')

args = parser.parse_args()

#setting the number of dataset.  
ds = 1       
if args.dataset == 2:
  ds = 2
  

#setting the connection to the database
mydb = mysql.connector.connect(
  host=args.input_host,             
  user=args.input_user,
  passwd=args.input_passwd,
  database=args.input_database
)

mycursor = mydb.cursor()

input_file_name = args.input_file



identical_lines = 0

prev_line = ""
in_file = open(input_file_name, "r")
for line in in_file:
   if prev_line =="":       #first line
      prev_line = line
      identical_lines = 1
   elif line == prev_line:             #identical lines
     identical_lines = identical_lines + 1
   else:                             #not identical lines
      prev_line2 = prev_line.strip()
      line2 = line.strip()
      fields = prev_line2.split(";")
      fields_new = line2.split(";")
      if fields[0] == fields_new[0] and fields[1] == fields_new[1] and  fields[3] != fields_new[3]:
         select = "SELECT mac_address FROM `mobiles` WHERE IP_ds"+str(ds)+" = \""+fields[0]+"\";"           
         mycursor.execute(select)
         myresult = mycursor.fetchone()
         mac_address = myresult[0]
         prev_line = line
         identical_lines = identical_lines +1
      elif fields[0] == fields_new[0] and fields[1] == fields_new[1] and  fields[2] != fields_new[2]:
         prev_line = line
         identical_lines = identical_lines + 1 
      else:         
         select = "SELECT mac_address FROM `mobiles` WHERE IP_ds"+str(ds)+" = \""+fields[0]+"\";"           
         mycursor.execute(select)
         myresult = mycursor.fetchone()
         mac_address = myresult[0]
         qry_length = len(fields[1])    
         if qry_length > 150:      #too long domain name in dns query
            dns_qry = fields[1][qry_length-150:]
         else:
            dns_qry = fields[1]
         s1 = "INSERT INTO `DNS_queries`(`dns_query`, `total_count_ds"+str(ds)+"`, `IP_with_qry_ds"+str(ds)+"`) VALUES (\""+dns_qry+"\", "+str(identical_lines)+", 1)" 
         s1 = s1 + "ON DUPLICATE KEY UPDATE `total_count_ds"+str(ds)+"` = `total_count_ds"+str(ds)+"` + " + str(identical_lines)+", `IP_with_qry_ds"+str(ds)+"` = `IP_with_qry_ds"+str(ds)+"` + 1 ;"
         s2 = "INSERT INTO `dataset"+str(ds)+"` (`mac_address`, `dns_query`, `total_count`) VALUES (\"" + mac_address + "\", \"" + dns_qry + "\", " + str(identical_lines) + ");"
         s3 = "UPDATE `mobiles` SET `number_of_qry_ds"+str(ds)+"` = `number_of_qry_ds"+str(ds)+"` + " + str(identical_lines)+", `diff_qry_ds"+str(ds)+"` = `diff_qry_ds"+str(ds)+"` + 1  WHERE mac_address=\""+mac_address+"\";"
         print (s1)
         print (s2)
         print (s3)
         prev_line = line
         identical_lines = 1
in_file.close()


    


