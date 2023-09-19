using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.Tablet;
using OpenTabletDriver.Plugin.Output;
using System;
using System.Numerics;

namespace BetterCalibrator {
[PluginName("BetterCalibrator")]
public sealed class BetterCalibratorCalibration : BetterCalibrator
{
/*     private Vector2 quadrant_stretch(Vector2 input)
    {
        switch (in_quadrant(input)) {
            case 1: {
                input.X *= right_stretch;
                input.Y *= top_stretch;
                break;
            }
            case 2: {
                input.X *= left_stretch;
                input.Y *= top_stretch;
                break;
            }
            case 3: {
                input.X *= left_stretch;
                input.Y *= bottom_stretch;
                break;
            }
            case 4: {
                input.X *= right_stretch;
                input.Y *= bottom_stretch;
                break;
            }
        }
        return input;
    }

    private Vector2 apply_offset(Vector2 input)
    {
        return new Vector2(input.X + x_offset, input.Y + y_offset);
    }

    public Vector2 filter(Vector2 input) {
        if (disable_clamping) {
            return from_unit_tablet(quadrant_stretch(to_unit_tablet(apply_offset(input))));
        }
        return from_unit_tablet(clamp(quadrant_stretch(to_unit_tablet(apply_offset(input)))));
    }
 */

    public override event Action<IDeviceReport> Emit;

    public override void Consume(IDeviceReport value)
    {
        if (value is ITabletReport report)
        {
            //report.Position = filter(report.Position);
            value = report;
        }

        Emit?.Invoke(value);
    }

    public override PipelinePosition Position => PipelinePosition.PreTransform;

    [Property("Left Stretch Multiplier"), DefaultPropertyValue(1f), ToolTip
        ("Tablet Calibration Stretch Tablet:\n\n" +
        "The multiplier used to stretch the left side of the tablet's X axis coordinates.")]
    public float left_stretch { set; get; }

    [Property("Right Stretch Multiplier"), DefaultPropertyValue(1f), ToolTip
        ("Tablet Calibration Stretch Tablet:\n\n" +
        "The multiplier used to stretch the right side of the tablet's X axis coordinates.")]
    public float right_stretch { set; get; }

    [Property("Top Stretch Multiplier"), DefaultPropertyValue(1f), ToolTip
        ("Tablet Calibration Stretch Tablet:\n\n" +
        "The multiplier used to stretch the top of the tablet's Y axis coordinates.")]
    public float top_stretch { set; get; }

    [Property("Bottom Stretch Multiplier"), DefaultPropertyValue(1f), ToolTip
        ("Tablet Calibration Stretch Tablet:\n\n" +
        "The multiplier used to stretch the bottom of the tablet's Y axis coordinates.")]
    public float bottom_stretch { set; get; }

    [Property("X Offset"), DefaultPropertyValue(0f), Unit("lines"), ToolTip
        ("Tablet Calibration Stretch Tablet:\n\n" +
        "The offset in lines used to move the center of the tablet's X axis coordinates.")]
    public float x_offset { set; get; }

    [Property("Y Offset"), DefaultPropertyValue(0f), Unit("lines"), ToolTip
        ("Tablet Calibration Stretch Tablet:\n\n" +
        "The offset in lines used to move the center of the tablet's Y axis coordinates.")]
    public float y_offset { set; get; }

    [BooleanProperty("Disable Clamping", ""), DefaultPropertyValue(true), ToolTip
        ("Tablet Calibration Stretch Tablet:\n\n" +
        "Allows the tablet coordinates to exceed the maximum tablet coordinates.")]
    public bool disable_clamping { set; get; }
}

}