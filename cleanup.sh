#!/bin/bash

# navigate to the directory you want to clean up
cd /Users/ramiemera/Documents/Rollbot/CoinFlip/SanTan

# delete all files named output.txt
rm -f output.txt

# check if input.txt exists
if [ ! -f input.txt ]; then
  # if it doesn't, duplicate inputBackup.txt and rename it to backup.txt
  cp inputBackup.txt input.txt
fi

# delete contents of data.txt
echo -n > data.txt