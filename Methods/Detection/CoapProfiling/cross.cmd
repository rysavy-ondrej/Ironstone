REM Create profiles for all datasets
dotnet Ironstone.Analyzers.CoapProfiling.dll Learn-Profile -InputFile=SampleData/idle.csv -WriteTo Profiles/idle30.profile -Aggregate="udp.srcport,udp.dstport" -WindowSize=30 -LearningWindows=20%
dotnet Ironstone.Analyzers.CoapProfiling.dll Learn-Profile -InputFile=SampleData/regular.csv -WriteTo Profiles/regular30.profile -Aggregate="udp.srcport,udp.dstport" -WindowSize=30 -LearningWindows=20%
dotnet Ironstone.Analyzers.CoapProfiling.dll Learn-Profile -InputFile=SampleData/observe.csv -WriteTo Profiles/observe30.profile -Aggregate="udp.srcport,udp.dstport" -WindowSize=30 -LearningWindows=20%
dotnet Ironstone.Analyzers.CoapProfiling.dll Learn-Profile -InputFile=SampleData/attack.csv -WriteTo Profiles/attack30.profile -Aggregate="udp.srcport,udp.dstport" -WindowSize=30 -LearningWindows=20%

REM DISTANCES
dotnet Ironstone.Analyzers.CoapProfiling.dll Compute-Distance -ProfileFile=Profiles/idle30.profile -ProfileFile=Profiles/regular30.profile > Dumps/idle-regular30.dist
dotnet Ironstone.Analyzers.CoapProfiling.dll Compute-Distance -ProfileFile=Profiles/idle30.profile -ProfileFile=Profiles/observe30.profile > Dumps/idle-observe30.dist
dotnet Ironstone.Analyzers.CoapProfiling.dll Compute-Distance -ProfileFile=Profiles/idle30.profile -ProfileFile=Profiles/attack30.profile > Dumps/idle-attack30.dist

dotnet Ironstone.Analyzers.CoapProfiling.dll Compute-Distance -ProfileFile=Profiles/regular30.profile -ProfileFile=Profiles/idle30.profile > Dumps/regular-idle30.dist
dotnet Ironstone.Analyzers.CoapProfiling.dll Compute-Distance -ProfileFile=Profiles/regular30.profile -ProfileFile=Profiles/observe30.profile > Dumps/regular-observe30.dist
dotnet Ironstone.Analyzers.CoapProfiling.dll Compute-Distance -ProfileFile=Profiles/regular30.profile -ProfileFile=Profiles/attack30.profile > Dumps/regular-attack30.dist

REM Dump profiles
dotnet Ironstone.Analyzers.CoapProfiling.dll Print-Profile -InputFile Profiles/idle30.profile > Dumps/idle30.profile.txt
dotnet Ironstone.Analyzers.CoapProfiling.dll Print-Profile -InputFile Profiles/regular30.profile > Dumps/regular30.profile.txt
dotnet Ironstone.Analyzers.CoapProfiling.dll Print-Profile -InputFile Profiles/observe30.profile > Dumps/observe30.profile.txt
dotnet Ironstone.Analyzers.CoapProfiling.dll Print-Profile -InputFile Profiles/attack30.profile > Dumps/attack30.profile.txt

REM Cross check datasets:
REM idle:
dotnet Ironstone.Analyzers.CoapProfiling.dll Test-Capture -ProfileFile=Profiles/idle30.profile  -InputFile=SampleData/idle.csv -Aggregate="udp.srcport,udp.dstport" -WindowSize=30 -TresholdFactor=0.8 > Dumps/idle_idle30.txt
dotnet Ironstone.Analyzers.CoapProfiling.dll Test-Capture -ProfileFile=Profiles/idle30.profile  -InputFile=SampleData/regular.csv -Aggregate="udp.srcport,udp.dstport" -WindowSize=30 -TresholdFactor=0.8 > Dumps/idle_regular30.txt
dotnet Ironstone.Analyzers.CoapProfiling.dll Test-Capture -ProfileFile=Profiles/idle30.profile  -InputFile=SampleData/observe.csv -Aggregate="udp.srcport,udp.dstport" -WindowSize=30 -TresholdFactor=0.8 > Dumps/idle_observe30.txt
dotnet Ironstone.Analyzers.CoapProfiling.dll Test-Capture -ProfileFile=Profiles/idle30.profile  -InputFile=SampleData/attack.csv -Aggregate="udp.srcport,udp.dstport" -WindowSize=30 -TresholdFactor=0.8 > Dumps/idle_attack30.txt

REM regular:
dotnet Ironstone.Analyzers.CoapProfiling.dll Test-Capture -ProfileFile=Profiles/regular30.profile  -InputFile=SampleData/idle.csv -Aggregate="udp.srcport,udp.dstport" -WindowSize=30 -TresholdFactor=0.8 > Dumps/regular_idle30.txt
dotnet Ironstone.Analyzers.CoapProfiling.dll Test-Capture -ProfileFile=Profiles/regular30.profile  -InputFile=SampleData/regular.csv -Aggregate="udp.srcport,udp.dstport" -WindowSize=30 -TresholdFactor=0.8 > Dumps/regular_regular30.txt
dotnet Ironstone.Analyzers.CoapProfiling.dll Test-Capture -ProfileFile=Profiles/regular30.profile  -InputFile=SampleData/observe.csv -Aggregate="udp.srcport,udp.dstport" -WindowSize=30 -TresholdFactor=0.8 > Dumps/regular_observe30.txt
dotnet Ironstone.Analyzers.CoapProfiling.dll Test-Capture -ProfileFile=Profiles/regular30.profile  -InputFile=SampleData/attack.csv -Aggregate="udp.srcport,udp.dstport" -WindowSize=30 -TresholdFactor=0.8 > Dumps/regular_attack30.txt

REM observe:
dotnet Ironstone.Analyzers.CoapProfiling.dll Test-Capture -ProfileFile=Profiles/observe30.profile  -InputFile=SampleData/idle.csv -Aggregate="udp.srcport,udp.dstport" -WindowSize=30 -TresholdFactor=0.8 > Dumps/observe_idle30.txt
dotnet Ironstone.Analyzers.CoapProfiling.dll Test-Capture -ProfileFile=Profiles/observe30.profile  -InputFile=SampleData/regular.csv -Aggregate="udp.srcport,udp.dstport" -WindowSize=30 -TresholdFactor=0.8 > Dumps/observe_regular30.txt
dotnet Ironstone.Analyzers.CoapProfiling.dll Test-Capture -ProfileFile=Profiles/observe30.profile  -InputFile=SampleData/observe.csv -Aggregate="udp.srcport,udp.dstport" -WindowSize=30 -TresholdFactor=0.8 > Dumps/observe_observe30.txt
dotnet Ironstone.Analyzers.CoapProfiling.dll Test-Capture -ProfileFile=Profiles/observe30.profile  -InputFile=SampleData/attack.csv -Aggregate="udp.srcport,udp.dstport" -WindowSize=30 -TresholdFactor=0.8 > Dumps/observe_attack30.txt

REM attack:
dotnet Ironstone.Analyzers.CoapProfiling.dll Test-Capture -ProfileFile=Profiles/attack30.profile  -InputFile=SampleData/idle.csv -Aggregate="udp.srcport,udp.dstport" -WindowSize=30 -TresholdFactor=0.8 > Dumps/attack_idle30.txt
dotnet Ironstone.Analyzers.CoapProfiling.dll Test-Capture -ProfileFile=Profiles/attack30.profile  -InputFile=SampleData/regular.csv -Aggregate="udp.srcport,udp.dstport" -WindowSize=30 -TresholdFactor=0.8 > Dumps/attack_regular30.txt
dotnet Ironstone.Analyzers.CoapProfiling.dll Test-Capture -ProfileFile=Profiles/attack30.profile  -InputFile=SampleData/observe.csv -Aggregate="udp.srcport,udp.dstport" -WindowSize=30 -TresholdFactor=0.8 > Dumps/attack_observe30.txt
dotnet Ironstone.Analyzers.CoapProfiling.dll Test-Capture -ProfileFile=Profiles/attack30.profile  -InputFile=SampleData/attack.csv -Aggregate="udp.srcport,udp.dstport" -WindowSize=30 -TresholdFactor=0.8 > Dumps/attack_attack30.txt

REM SINGLE MODEL:
dotnet Ironstone.Analyzers.CoapProfiling.dll Test-Capture -ProfileFile=Profiles/idle30.profile -ProfileFile=Profiles/regular30.profile -InputFile=SampleData/idle.csv -Aggregate="udp.srcport,udp.dstport" -WindowSize=30 -TresholdFactor=0.8 > Dumps/idle_regular_idle30.txt
dotnet Ironstone.Analyzers.CoapProfiling.dll Test-Capture -ProfileFile=Profiles/idle30.profile -ProfileFile=Profiles/regular30.profile -InputFile=SampleData/regular.csv -Aggregate="udp.srcport,udp.dstport" -WindowSize=30 -TresholdFactor=0.8 > Dumps/idle_regular_regular30.txt
dotnet Ironstone.Analyzers.CoapProfiling.dll Test-Capture -ProfileFile=Profiles/idle30.profile -ProfileFile=Profiles/regular30.profile -InputFile=SampleData/observe.csv -Aggregate="udp.srcport,udp.dstport" -WindowSize=30 -TresholdFactor=0.8 > Dumps/idle_regular_observe30.txt
dotnet Ironstone.Analyzers.CoapProfiling.dll Test-Capture -ProfileFile=Profiles/idle30.profile -ProfileFile=Profiles/regular30.profile -InputFile=SampleData/attack.csv -Aggregate="udp.srcport,udp.dstport" -WindowSize=30 -TresholdFactor=0.8 > Dumps/idle_regular_attack30.txt

dotnet Ironstone.Analyzers.CoapProfiling.dll Test-Capture -ProfileFile=Profiles/idle30.profile -ProfileFile=Profiles/regular30.profile -ProfileFile=Profiles/observe30.profile -InputFile=SampleData/idle.csv -Aggregate="udp.srcport,udp.dstport" -WindowSize=30 -TresholdFactor=0.8 > Dumps/all_idle30.txt
dotnet Ironstone.Analyzers.CoapProfiling.dll Test-Capture -ProfileFile=Profiles/idle30.profile -ProfileFile=Profiles/regular30.profile -ProfileFile=Profiles/observe30.profile -InputFile=SampleData/regular.csv -Aggregate="udp.srcport,udp.dstport" -WindowSize=30 -TresholdFactor=0.8 > Dumps/all_regular30.txt
dotnet Ironstone.Analyzers.CoapProfiling.dll Test-Capture -ProfileFile=Profiles/idle30.profile -ProfileFile=Profiles/regular30.profile -ProfileFile=Profiles/observe30.profile -InputFile=SampleData/observe.csv -Aggregate="udp.srcport,udp.dstport" -WindowSize=30 -TresholdFactor=0.8 > Dumps/all_observe30.txt
dotnet Ironstone.Analyzers.CoapProfiling.dll Test-Capture -ProfileFile=Profiles/idle30.profile -ProfileFile=Profiles/regular30.profile -ProfileFile=Profiles/observe30.profile -InputFile=SampleData/attack.csv -Aggregate="udp.srcport,udp.dstport" -WindowSize=30 -TresholdFactor=0.8 > Dumps/all_attack30.txt




