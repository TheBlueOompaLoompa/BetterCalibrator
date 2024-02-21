# Better Calibrator
This is meant to make calibration easier. I had enough time on my hands to make this and I would rather have a simpler calibration process. Manually adjusting numbers in a text box is usable, but not user friendly or quick. I need to recalibrate my tablet so often with constantly changing conditions and locations. This plugin was made by using [PSSA](https://github.com/jamesbt365/PSSA) as a template for getting started with plugins, thanks to jamesbt365. Also, thanks to [Tablet_Calibration](https://github.com/Kuuuube/Tablet_Calibration/tree/main) by Kuuuube for code that converts tablet units.

## How to Use
Immediately after installing the plugin through OpenTabletDriver, click on the `Filters` tab and disable BetterCalibrator.
Then, go to the directory where it's installed.
### Linux
- Usually found in: `~/.config/OpenTabletDriver/Plugin/BetterCalibrator`
- Make calibrator.x86_64 an executable \(Through properties or chmod +x\)
- Launch it and complete the calibration by tapping the center of each target with your pen.
### Windows
- Press Win+R, the "run" window will come up, input the following directory: `%localappdata%\OpenTabletDriver\Plugin\BetterCalibrator` and press enter
- The plugin's directory will open up, simply run `calibrator.exe` and complete the calibration by tapping the center of each target with your pen.

After you click the last target, the calibrator program will save the configuration. Now you can re-enable the filter. The configuration will automatically load and you will get calibrated output.


## Supported Platforms

- [x] Windows
- [x] Linux
- [ ] MacOS

I can't compile for Mac as I don't own any apple computers.

Windows support is new, I don't have a machine to test this on so please send in any issues you may have.
