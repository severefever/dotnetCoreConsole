#!/bin/bash
# Всего
totalMemory=$(grep -Po 'MemTotal: *\K[0-9]+' /proc/meminfo | awk '{print int($0/1024+0.5)}')
# Доступно
availableMemory=$(grep -Po 'MemAvailable: *\K[0-9]+' /proc/meminfo | awk '{print int($0/1024+0.5)}')
# occupied
let usedMemory=${totalMemory}-${availableMemory}
echo "${totalMemory%$'\r'}"
echo "${availableMemory%$'\r'}"
echo "${usedMemory%$'\r'}"