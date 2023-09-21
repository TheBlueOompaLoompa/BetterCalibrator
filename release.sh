#!/bin/bash
dotnet build --configuration Release

rm BetterCalibrator.zip

mkdir tmp
cd tmp

cp ../BetterCalibrator/bin/Release/net6.0/BetterCalibrator.dll .
cp ../BetterCalibratorPopup/calibrator.x86_64 .

zip BetterCalibrator.zip BetterCalibrator.dll calibrator.x86_64

mv BetterCalibrator.zip ../

cd ..
rm -r tmp

sha256sum BetterCalibrator.zip