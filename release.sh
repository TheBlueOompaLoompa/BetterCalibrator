#!/usr/bin/env /bin/bash
if [ $# -eq 0 ]
  then
    echo "No arguments supplied, need version"
    exit 1
fi

echo Bulding BetterCalibrator Release
dotnet build --configuration Release

rm BetterCalibrator-*.zip

TMP=/tmp/BetterCalibrator

mkdir $TMP
mkdir $TMP/win
mkdir $TMP/mac
mkdir $TMP/lin

PRJ=$(pwd)

# Build Windows
echo Windows Build
godot --headless --path ./BetterCalibratorPopup --export-release "Windows Desktop" $TMP/win/calibrator.exe
cp BetterCalibrator/bin/Release/net8.0/BetterCalibrator.dll $TMP/win
cd $TMP/win
zip -r $PRJ/BetterCalibrator-windows.zip *
cd $PRJ

# Build Mac
echo Mac Build
godot --headless --path ./BetterCalibratorPopup --export-release "macOS" $TMP/mac/calibrator.app
cp BetterCalibrator/bin/Release/net8.0/BetterCalibrator.dll $TMP/mac
cd $TMP/mac
zip -r $PRJ/BetterCalibrator-mac.zip *
cd $PRJ

# Build Linux
echo Linux Build
godot --headless --path ./BetterCalibratorPopup --export-release "Linux/X11" $TMP/lin/calibrator.x86_64
cp BetterCalibrator/bin/Release/net8.0/BetterCalibrator.dll $TMP/lin
cd $TMP/lin
zip -r $PRJ/BetterCalibrator-linux.zip *
cd $PRJ

rm -r $TMP

# Generate metadata files
echo Generating Metadata
WIN=$(sha256sum BetterCalibrator-windows.zip)
MAC=$(sha256sum BetterCalibrator-mac.zip)
LIN=$(sha256sum BetterCalibrator-linux.zip)

IFS=' ' read -ra ADDR <<< "$WIN"
WIN=${ADDR[0]}
IFS=' ' read -ra ADDR <<< "$MAC"
MAC=${ADDR[0]}
IFS=' ' read -ra ADDR <<< "$LIN"
LIN=${ADDR[0]}

rm BetterCalibrator-*.json

cp BetterCalibrator.json BetterCalibrator-win.json
cp BetterCalibrator.json BetterCalibrator-mac.json
cp BetterCalibrator.json BetterCalibrator-lin.json

sed -i "s/PLATFORM/Windows/g" BetterCalibrator-win.json
sed -i "s/REPLACE_SUM/$WIN/g" BetterCalibrator-win.json

sed -i "s/PLATFORM/macOS/g" BetterCalibrator-mac.json
sed -i "s/REPLACE_SUM/$MAC/g" BetterCalibrator-mac.json

sed -i "s/PLATFORM/Linux/g" BetterCalibrator-lin.json
sed -i "s/REPLACE_SUM/$LIN/g" BetterCalibrator-lin.json

sed -i "s/REPLACE_VERSION/$1/g" BetterCalibrator-*.json
