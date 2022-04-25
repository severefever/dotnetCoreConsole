#!/bin/bash
# Всего
totalMemory=$(grep -Po 'MemTotal: *\K[0-9]+' /proc/meminfo | awk '{print int($0/1024+0.5)}')
#'{print int($1+0.5)}'
# Доступно
availableMemory=$(grep -Po 'MemAvailable: *\K[0-9]+' /proc/meminfo | awk '{print int($0/1024+0.5)}')
# occupied
let occupiedMemory=${totalMemory}-${availableMemory}
echo "Всего оперативной памяти: ${totalMemory%$'\r'} Mb"
echo "Свободно: ${availableMemory%$'\r'} Mb"
echo "Занято: ${occupiedMemory%$'\r'} Mb"