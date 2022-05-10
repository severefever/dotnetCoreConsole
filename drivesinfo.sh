#!/bin/bash
drives=$(lsblk -id | grep -i 'sd' | awk '{print "/dev/"$1}')
for drive in $drives
do
  type=$(lsblk -ido TYPE $drive | grep -v '^TYPE')
  instanceName=$drive
  driveFormat=$(df -Th $drive | grep -vi 'filesystem\|файл.система' | tr -s ' ' | cut -d' ' -f2)
  label=$(df -h $drive | grep -vi 'filesystem\|файл.система' | cut -d' ' -f1)
  totalFreeSpace=$(df -hBG $drive | grep -vi 'filesystem\|файл.система' | tr -s ' ' | cut -d' ' -f4 | sed 's/[^0-9.]//g')
  totalSize=$(df -hBG $drive | grep -vi 'filesystem\|файл.система' | tr -s ' ' | cut -d' ' -f2 | sed 's/[^0-9.]//g')
  usedSpace=$(df -hBG $drive | grep -vi 'filesystem\|файл.система' | tr -s ' ' | cut -d' ' -f3 | sed 's/[^0-9.]//g')
  echo "$label $type $driveFormat $totalFreeSpace $totalSize $usedSpace $instanceName"
done