#!/bin/bash
# Количество ядер процессора (physical cores)
physicalCpuCount=$(lscpu -p | egrep -v '^#' | sort -u -t, -k 2,4 | wc -l)
# Количество потоков процессора (logical cores)
logicalCpuCount=$(lscpu -p | egrep -v '^#' | wc -l)
# Model name
modelName=$(cat /proc/cpuinfo | egrep -i 'model name' | tr -s ' ' | cut -d' ' -f3-)
# Current clock speed
currentSpeed=$(cat /proc/cpuinfo | egrep -i 'cpu mhz' | tr -s ' ' | cut -d' ' -f3- | uniq)
# Current load percentage
loadPercentage=$(awk '{u=$2+$4; t=$2+$4+$5; if (NR==1){u1=u; t1=t;} else printf "%.1f", ($2+$4-u1) * 100 / (t-t1) "%"; }' <(grep 'cpu ' /proc/stat) <(sleep 0.5;grep 'cpu ' /proc/stat))
echo "model name: ${modelName%$'\r'}"
echo "current clock speed: ${currentSpeed%$'\r'} Mhz"
echo "physical cores: ${physicalCpuCount%$'\r'}"
echo "logical cores: ${logicalCpuCount%$'\r'}"
echo "current load: ${loadPercentage%$'\r'}%"