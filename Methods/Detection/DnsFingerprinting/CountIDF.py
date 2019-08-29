
# Created in July 2019 by Ivana Burgetova
# Project: DNS Profiling

"""
This script computes the IDF value for the DNS_queries in the provided database.
For each DNS_query the SQL command UPDATE is generated, so the IDF values can be stored in the database.
The IDF value is treated separately for each dataset.
The natural logarithm is used for IDF value.
"""

from math import log
import mysql.connector
from argparse import ArgumentParser

#definition of the arguments
parser = ArgumentParser(description='The arguments allow to specify the details for database connection.')  
parser.add_argument("-i","--input_host", required=True, help='server name or IP address on which MySQL is running')
parser.add_argument("-u","--input_user", required=True, help='the username that is used to work with MySQL Server')
parser.add_argument("-p","--input_passwd", required=True, help='the password that is used to work with MySQL Server')
parser.add_argument("-d","--input_database", required=True, help='database name to which you want to connect')

args = parser.parse_args()


#setting the connection to the database
mydb = mysql.connector.connect(
  host=args.input_host,             
  user=args.input_user,
  passwd=args.input_passwd,
  database=args.input_database
)

#number of communicating devices in each dataset
number_of_devices_ds1 = 13
number_of_devices_ds2 = 7

mycursor = mydb.cursor()

#for each DNS_query from the database, IDFs are calculated and SQL query is created
select = "SELECT `dns_query`, `IP_with_qry_ds1`, `IP_with_qry_ds2` FROM `DNS_queries`;"           
mycursor.execute(select)
myresult = mycursor.fetchall()
for x in myresult:
   IDF_ds1 = float(0)
   if x[1] != 0:
      IDF_ds1 = float (number_of_devices_ds1) / x[1]  
      IDF_ds1 = log (IDF_ds1) 
   update = "UPDATE `DNS_queries` SET IDF_ds1="+str(IDF_ds1)

   IDF_ds2 = float(0)
   if x[2] != 0:
      IDF_ds2 = float (number_of_devices_ds2) / x[2]  
      IDF_ds2 = log (IDF_ds2) 
   update = update + ", IDF_ds2="+str(IDF_ds2)

   update = update + " WHERE `dns_query` =\""+x[0]+"\";"
   print (update)

   
