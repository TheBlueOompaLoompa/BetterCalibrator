pkill -f OpenTabletDriver.Daemon
pkill -f OpenTabletDriver.UX.Gtk

dotnet build

mkdir -p ~/.config/OpenTabletDriver/Plugins/BetterCalibrator/
cp BetterCalibrator/bin/Debug/net8.0/BetterCalibrator.dll ~/.config/OpenTabletDriver/Plugins/BetterCalibrator/BetterCalibrator.dll
cp BetterCalibrator/bin/Debug/net8.0/calibrator.x86_64 ~/.config/OpenTabletDriver/Plugins/BetterCalibrator/calibrator.x86_64

otd-gui > /dev/null 2>&1 & otd-daemon
