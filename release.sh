#!/bin/bash
dotnet build --configuration Release

rm BetterCalibrator.zip

mkdir tmp
cd tmp

godot --headless --path ../BetterCalibratorPopup --export-release "Windows Desktop" ../tmp/calibrator.exe
godot --headless --path ../BetterCalibratorPopup --export-release "Linux/X11" ../tmp/calibrator.x86_64

cp ../BetterCalibrator/bin/Release/net6.0/BetterCalibrator.dll .

zip BetterCalibrator.zip BetterCalibrator.dll calibrator.x86_64 calibrator.exe

mv BetterCalibrator.zip ../

cd ..
rm -r tmp

sha256sum BetterCalibrator.zip
