#!/bin/bash
# Количество ядер процессора (physical cores)
physicalCpuCount=$(lscpu -p | egrep -v '^#' | sort -u -t, -k 2,4 | wc -l)
# Количество потоков процессора (logical cores)
logicalCpuCount=$(lscpu -p | egrep -v '^#' | wc -l)
# Название модели
modelName=$(cat /proc/cpuinfo | egrep -i 'model name' | tr -s ' ' | cut -d' ' -f3- | uniq)
# Текущая частота процессора
currentSpeed=$(cat /proc/cpuinfo | egrep -i 'cpu mhz' | tr -s ' ' | cut -d' ' -f3- | awk '{print int($0+0.5)}' | uniq)
# Текущая загрузка процессора
loadPercentage=$(top -bn1 | grep "Cpu(s)" | sed -r 's/\,([0-9]{1,2})\b/.\1/g' |  sed "s/.*, *\([0-9.]*\)%* id.*/\1/" | awk '{print 100 - $1}')
echo "${modelName%$'\r'}"
echo "${currentSpeed%$'\r'}"
echo "${physicalCpuCount%$'\r'}"
echo "${logicalCpuCount%$'\r'}"
echo "${loadPercentage%$'\r'}"
# Температура
cpuTemp=$(sensors | egrep -i 'Package id' | awk '{print $4}' | sed 's/[^0-9.]//g' | uniq)
echo "${cpuTemp%$'\r'}"