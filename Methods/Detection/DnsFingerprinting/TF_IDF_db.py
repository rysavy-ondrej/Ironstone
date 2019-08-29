#!/usr/bin/env python

# Created in July 2019 by Ivana Burgetova
# Project: DNS Profiling

"""
This script computes the score between pairs of DNS fingerprints using TF-IDF methodology.
Fingerprints are extracted from the provided database that contains two datasets.
One dataset is used as source dataset - each fingerprint from this dataset is matched against all fingerprints from the second dataset. 
The score of each pair of fingerprints is print out to the standard output.
The source dataset can be specified by the argument -s.

"""



from math import sqrt
import mysql.connector
from argparse import ArgumentParser

#definition of the arguments
parser = ArgumentParser(description='The arguments allow to specify the details for database connection and the source and target datasets.')   
parser.add_argument("-i","--input_host", required=True, help='server name or IP address on which MySQL is running')
parser.add_argument("-u","--input_user", required=True, help='the username that is used to work with MySQL Server')
parser.add_argument("-p","--input_passwd", required=True, help='the password that is used to work with MySQL Server')
parser.add_argument("-d","--input_database", required=True, help='database name to which you want to connect')
parser.add_argument("-s","--source_dataset", type=int, default = 1, help='fingerprints from this dataset are matched against fingerprints from the second dataset')


args = parser.parse_args()

#setting the source and target dataset. Fingeprints from the source dataset are matched against fingerprints from the target dataset
ds_source = 1       
ds_target = 2
if args.source_dataset == 2:
  ds_source = 2
  ds_target = 1

#setting the connection to the database
mydb = mysql.connector.connect(
  host=args.input_host,             
  user=args.input_user,
  passwd=args.input_passwd,
  database=args.input_database
)

mycursor = mydb.cursor()

#dotaz = source
#dokument = target


#extraction of the devices (fingerprint) from the source dataset
select = "SELECT `mac_address`, `IP_ds"+str(ds_source)+"`, `number_of_qry_ds"+str(ds_source)+"` FROM `mobiles` WHERE `number_of_qry_ds"+str(ds_source)+"` !=0;"           
mycursor.execute(select)
myresult = mycursor.fetchall()
for x in myresult:        # for each device (fingerprint) from the source dataset
   mac_source = x[0]
   list_of_results = list()
   #extraction of the devices (fingerprints) from the target dataset
   select2 = "SELECT `mac_address`, `IP_ds"+str(ds_target)+"`, `number_of_qry_ds"+str(ds_target)+"` FROM `mobiles` WHERE `number_of_qry_ds"+str(ds_target)+"` !=0;"
   mycursor.execute(select2)
   myresult2 = mycursor.fetchall()
   for y in myresult2:     #for each device (fingerprint) from the target dataset
     mac_target = y[0]
     #set up the data structures for TF-IDF score
     TF_source = dict()
     TF_target = dict()
     IDF_source = dict()
     IDF_target = dict()
     TF_IDF_total = 0.0
     source_size = 0.0
     target_size = 0.0
     select3 = "SELECT dns_query FROM `dataset"+str(ds_source)+"` WHERE dataset"+str(ds_source)+".mac_address=\""+str(mac_source)+"\" UNION SELECT dns_query FROM `dataset"+str(ds_target)+"` WHERE dataset"+str(ds_target)+".mac_address=\""+str(mac_target)+"\";"
     mycursor.execute(select3)
     myresult3 = mycursor.fetchall()
     for z in myresult3:
        TF_source[z[0]] = 0 
        TF_target[z[0]] = 0               
     max_frequency_source = 0
     max_frequency_target = 0
     #extract the frequencies of the selected DNS queries for both devices
     select4 = "SELECT dns_query, total_count FROM `dataset"+str(ds_source)+"` WHERE mac_address=\""+str(mac_source)+"\";"
     mycursor.execute(select4)
     myresult4 = mycursor.fetchall()
     for q in myresult4:
         TF_source[q[0]] = q[1]
         if q[1] > max_frequency_source:
            max_frequency_source = q[1] 
     select5 = "SELECT dns_query, total_count FROM `dataset"+str(ds_target)+"` WHERE mac_address=\""+str(mac_target)+"\";"
     mycursor.execute(select5)
     myresult5 = mycursor.fetchall()
     for d in myresult5:
         TF_target[d[0]] = d[1] 
         if d[1] > max_frequency_target:
            max_frequency_target = d[1] 
     #TF calculation
     for z in myresult3:
        if TF_source [z[0]] != 0:
           TF_source [z[0]] = 0.5 + (0.5*TF_source[z[0]]/max_frequency_source)
        if TF_target [z[0]] != 0:   
           TF_target [z[0]] = 0.5 + (0.5*TF_target[z[0]]/max_frequency_target)
        #extract IDF values from the database
        select6 = "SELECT `IDF_ds"+str(ds_target)+"` FROM `DNS_queries` WHERE `DNS_query`=\""+z[0]+"\";"
        mycursor.execute(select6)
        myresult6 = mycursor.fetchone()
        IDF_source[z[0]] = myresult6[0]
        select7 = "SELECT `IDF_ds"+str(ds_target)+"` FROM `DNS_queries` WHERE `DNS_query`=\""+z[0]+"\";"
        mycursor.execute(select7)
        myresult7 = mycursor.fetchone()
        IDF_target[z[0]] = myresult7[0]
        #TF-IDF calculation
        TF_IDF_total = TF_IDF_total + (TF_source[z[0]]*IDF_source[z[0]]*TF_target[z[0]]*IDF_target[z[0]])
        source_size = source_size + ((TF_source[z[0]]*IDF_source[z[0]])**2)
        target_size = target_size + ((TF_target[z[0]]*IDF_target[z[0]])**2)
     source_size = sqrt(source_size)
     target_size = sqrt(target_size)
     if source_size !=0 and target_size !=0:
        TF_IDF_total = TF_IDF_total/(source_size*target_size)   
     #storing the score for the given pair of fingeprints
     list_of_results.append([TF_IDF_total, mac_source+" + "+mac_target])
   #sorting of the results for the given source fingeprint  
   list_of_results.sort()    
   #print the results 
   for score, mac in list_of_results:        
      print mac, score
   output = "******************************************************"
   print(output)