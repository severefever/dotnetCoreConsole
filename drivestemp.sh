#!/bin/bash
drives=$(lsblk -id | grep -i 'sd' | awk '{print "/dev/"$1}')
for drive in $drives
do
  temperature=$(hddtemp --numeric $drive | sed 's/[^0-9]//g')
  model=$(hddtemp --separator='|' $drive | awk -F'|' '{print $3}')
  echo "$model $temperature $drive"
done