#!/bin/bash
dotnet build --configuration Release

rm BetterCalibrator.zip

mkdir tmp
cd tmp

cp ../BetterCalibrator/bin/Release/net6.0/BetterCalibrator.dll .
cp ../BetterCalibratorPopup/calibrator.x86_64 .
cp ../BetterCalibratorPopup/calibrator.x86_64.exe .

zip BetterCalibrator.zip BetterCalibrator.dll calibrator.x86_64 calibrator.x86_64.exe

mv BetterCalibrator.zip ../

cd ..
rm -r tmp

sha256sum BetterCalibrator.zip