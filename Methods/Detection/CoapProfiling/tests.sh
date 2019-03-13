#!/bin/bash

#WINDOW=30
dotnet Ironstone.Analyzers.CoapProfiling.dll Learn-Profile -InputFile=SampleData/coap-learn.csv -WriteTo coap30.profile -Aggregate="udp.srcport,udp.dstport" -WindowSize=30
dotnet Ironstone.Analyzers.CoapProfiling.dll Print-Profile -InputFile coap30.profile > coap30_profile.txt
dotnet Ironstone.Analyzers.CoapProfiling.dll Test-Capture -ProfileFile=coap30.profile  -InputFile=../../../SampleData/coap-test.csv -Aggregate="udp.srcport,udp.dstport" -WindowSize=15 > coap30_test15.txt
dotnet Ironstone.Analyzers.CoapProfiling.dll Test-Capture -ProfileFile=coap30.profile  -InputFile=../../../SampleData/coap-test.csv -Aggregate="udp.srcport,udp.dstport" -WindowSize=30 > coap30_test30.txt
dotnet Ironstone.Analyzers.CoapProfiling.dll Test-Capture -ProfileFile=coap30.profile  -InputFile=../../../SampleData/coap-test.csv -Aggregate="udp.srcport,udp.dstport" -WindowSize=60 > coap30_test60.txt

# WINDOW=60
dotnet Ironstone.Analyzers.CoapProfiling.dll Learn-Profile -InputFile=SampleData/coap-learn.csv -WriteTo coap60.profile -Aggregate="udp.srcport,udp.dstport" -WindowSize=60
dotnet Ironstone.Analyzers.CoapProfiling.dll Print-Profile -InputFile coap60.profile  > coap60_profile.txt
dotnet Ironstone.Analyzers.CoapProfiling.dll Test-Capture -ProfileFile=coap60.profile  -InputFile=../../../SampleData/coap-test.csv -Aggregate="udp.srcport,udp.dstport" -WindowSize=30 > coap60_test30.txt
dotnet Ironstone.Analyzers.CoapProfiling.dll Test-Capture -ProfileFile=coap60.profile  -InputFile=../../../SampleData/coap-test.csv -Aggregate="udp.srcport,udp.dstport" -WindowSize=60 > coap60_test60.txt
dotnet Ironstone.Analyzers.CoapProfiling.dll Test-Capture -ProfileFile=coap60.profile  -InputFile=../../../SampleData/coap-test.csv -Aggregate="udp.srcport,udp.dstport" -WindowSize=90 > coap60_test90.txt

# WINDOW=90
dotnet Ironstone.Analyzers.CoapProfiling.dll Learn-Profile -InputFile=SampleData/coap-learn.csv -WriteTo coap90.profile -Aggregate="udp.srcport,udp.dstport" -WindowSize=90
dotnet Ironstone.Analyzers.CoapProfiling.dll Print-Profile -InputFile coap90.profile  > coap90_profile.txt
dotnet Ironstone.Analyzers.CoapProfiling.dll Test-Capture -ProfileFile=coap90.profile  -InputFile=../../../SampleData/coap-test.csv -Aggregate="udp.srcport,udp.dstport" -WindowSize=60 > coap90_test60.txt
dotnet Ironstone.Analyzers.CoapProfiling.dll Test-Capture -ProfileFile=coap90.profile  -InputFile=../../../SampleData/coap-test.csv -Aggregate="udp.srcport,udp.dstport" -WindowSize=90 > coap90_test90.txt
dotnet Ironstone.Analyzers.CoapProfiling.dll Test-Capture -ProfileFile=coap90.profile  -InputFile=../../../SampleData/coap-test.csv -Aggregate="udp.srcport,udp.dstport" -WindowSize=120 > coap90_test120.txt

# WINDOW=120
dotnet Ironstone.Analyzers.CoapProfiling.dll Learn-Profile -InputFile=SampleData/coap-learn.csv -WriteTo coap120.profile -Aggregate="udp.srcport,udp.dstport" -WindowSize=120
dotnet Ironstone.Analyzers.CoapProfiling.dll Print-Profile -InputFile coap120.profile  > coap120_profile.txt
dotnet Ironstone.Analyzers.CoapProfiling.dll Test-Capture -ProfileFile=coap120.profile  -InputFile=../../../SampleData/coap-test.csv -Aggregate="udp.srcport,udp.dstport" -WindowSize=90 > coap120_test90.txt
dotnet Ironstone.Analyzers.CoapProfiling.dll Test-Capture -ProfileFile=coap120.profile  -InputFile=../../../SampleData/coap-test.csv -Aggregate="udp.srcport,udp.dstport" -WindowSize=120 > coap120_test120.txt
dotnet Ironstone.Analyzers.CoapProfiling.dll Test-Capture -ProfileFile=coap120.profile  -InputFile=../../../SampleData/coap-test.csv -Aggregate="udp.srcport,udp.dstport" -WindowSize=150 > coap120_test150.txt

# WINDOW=150
dotnet Ironstone.Analyzers.CoapProfiling.dll Learn-Profile -InputFile=SampleData/coap-learn.csv -WriteTo coap150.profile -Aggregate="udp.srcport,udp.dstport" -WindowSize=150
dotnet Ironstone.Analyzers.CoapProfiling.dll Print-Profile -InputFile coap150.profile  > coap150_profile.txt
dotnet Ironstone.Analyzers.CoapProfiling.dll Test-Capture -ProfileFile=coap150.profile  -InputFile=../../../SampleData/coap-test.csv -Aggregate="udp.srcport,udp.dstport" -WindowSize=120 > coap150_test120.txt
dotnet Ironstone.Analyzers.CoapProfiling.dll Test-Capture -ProfileFile=coap150.profile  -InputFile=../../../SampleData/coap-test.csv -Aggregate="udp.srcport,udp.dstport" -WindowSize=150 > coap150_test150.txt
dotnet Ironstone.Analyzers.CoapProfiling.dll Test-Capture -ProfileFile=coap150.profile  -InputFile=../../../SampleData/coap-test.csv -Aggregate="udp.srcport,udp.dstport" -WindowSize=180 > coap150_test180.txt

# WINDOW=180
dotnet Ironstone.Analyzers.CoapProfiling.dll Learn-Profile -InputFile=SampleData/coap-learn.csv -WriteTo coap180.profile -Aggregate="udp.srcport,udp.dstport" -WindowSize=180
dotnet Ironstone.Analyzers.CoapProfiling.dll Print-Profile -InputFile coap180.profile  > coap180_profile.txt
dotnet Ironstone.Analyzers.CoapProfiling.dll Test-Capture -ProfileFile=coap180.profile  -InputFile=../../../SampleData/coap-test.csv -Aggregate="udp.srcport,udp.dstport" -WindowSize=150 > coap180_test150.txt
dotnet Ironstone.Analyzers.CoapProfiling.dll Test-Capture -ProfileFile=coap180.profile  -InputFile=../../../SampleData/coap-test.csv -Aggregate="udp.srcport,udp.dstport" -WindowSize=180 > coap180_test150.txt
dotnet Ironstone.Analyzers.CoapProfiling.dll Test-Capture -ProfileFile=coap180.profile  -InputFile=../../../SampleData/coap-test.csv -Aggregate="udp.srcport,udp.dstport" -WindowSize=210 > coap180_test210.txt