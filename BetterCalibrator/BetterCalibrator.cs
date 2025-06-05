using System.Numerics;
using System.Reflection;
using System.Text.Json;
using OpenTabletDriver;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.Plugin.DependencyInjection;
using OpenTabletDriver.Plugin.Output;
using OpenTabletDriver.Plugin.Tablet;

namespace BetterCalibrator;

[PluginName("BetterCalibrator")]
public class BetterCalibrator : IPositionedPipelineElement<IDeviceReport> {
    [Resolved]
    public IDriver driver;

    OffsetInfo info = new OffsetInfo();

    /*[Property("Calibration Data"), DefaultPropertyValue("{\"cols\":5,\"offsets\":[[3.5,-26],[5.5,-26],[5.5,-27],[5.5,-25],[8,-27],[4.5,-26.5],[4.5,-29.5],[5.5,-25.5],[3.5,-26.5],[388,-25.5],[5.5,-26.5],[5.5,-25.5],[5.5,-23.5],[2.5,-21.5],[0,-25.5]],\"rows\":3}")]
    public string Data {
        get; set;
    }*/

    public Vector2 to_unit_screen(Vector2 input)
    {
        if (output_mode_type == OutputModeType.absolute && absolute_output_mode != null)
        {
            var display = absolute_output_mode.Output;
            var offset = absolute_output_mode.Output.Position;
            var shiftoffX = offset.X - (display.Width / 2);
            var shiftoffY = offset.Y - (display.Height / 2);
            return new Vector2((input.X - shiftoffX) / display.Width * 2 - 1, (input.Y - shiftoffY) / display.Height * 2 - 1);
        }

        try_resolve_output_mode();
        return default;
    }

    public Vector2 from_unit_screen(Vector2 input)
    {
        if (output_mode_type == OutputModeType.absolute && absolute_output_mode != null)
        {
            var display = absolute_output_mode.Output;
            var offset = absolute_output_mode.Output.Position;
            var shiftoffX = offset.X - (display.Width / 2);
            var shiftoffY = offset.Y - (display.Height / 2);
            return new Vector2((input.X + 1) / 2 * display.Width + shiftoffX, (input.Y + 1) / 2 * display.Height + shiftoffY);
        }

        try_resolve_output_mode();
        return default;
    }

    public Vector2 to_unit_tablet(Vector2 input)
    {
        if (output_mode_type == OutputModeType.absolute && absolute_output_mode != null)
        {
            return new Vector2(input.X / absolute_output_mode.Tablet.Properties.Specifications.Digitizer.MaxX * 2 - 1, input.Y / absolute_output_mode.Tablet.Properties.Specifications.Digitizer.MaxY * 2 - 1);
        }

        try_resolve_output_mode();
        return default;
    }

    public Vector2 from_unit_tablet(Vector2 input)
    {
        if (output_mode_type == OutputModeType.absolute && absolute_output_mode != null)
        {
            return new Vector2((input.X + 1) / 2 * absolute_output_mode.Tablet.Properties.Specifications.Digitizer.MaxX, (input.Y + 1) / 2 * absolute_output_mode.Tablet.Properties.Specifications.Digitizer.MaxY);
        }

        try_resolve_output_mode();
        return default;
    }

    public Vector2 clamp(Vector2 input)
    {
        return new Vector2(
        Math.Clamp(input.X, -1, 1),
        Math.Clamp(input.Y, -1, 1)
        );
    }

    private bool parsed = false;

    private OutputModeType output_mode_type;
    private AbsoluteOutputMode absolute_output_mode;
    private RelativeOutputMode relative_output_mode;
    private void try_resolve_output_mode() {
        if(!parsed) {
            parsed = true;
            try {
                using(StreamReader reader = new StreamReader(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/offsets.json")) {
                    string json = reader.ReadToEnd();
                    info = JsonSerializer.Deserialize<OffsetInfo>(json);
                }
            } catch (Exception exception) {
                Log.Exception(exception);
            }
            
        }
        if (driver is Driver drv)
        {
            IOutputMode output = drv.InputDevices
                .Where(dev => dev?.OutputMode?.Elements?.Contains(this) ?? false)
                .Select(dev => dev?.OutputMode).FirstOrDefault();

            if (output is AbsoluteOutputMode abs_output) {
                absolute_output_mode = abs_output;
                output_mode_type = OutputModeType.absolute;
                return;
            }
            if (output is RelativeOutputMode rel_output) {
                relative_output_mode = rel_output;
                output_mode_type = OutputModeType.relative;
                return;
            }
            output_mode_type = OutputModeType.unknown;
        }
    }

    private Vector2 to_pixel(Vector2 input) {
        var screen = to_unit_screen(input);
        if(output_mode_type == OutputModeType.absolute && absolute_output_mode != null) {
            var display = absolute_output_mode.Output;
            
            screen.X += 1f;
            screen.Y += 1f;
            screen /= 2f;

            screen.X *= display.Width;
            screen.Y *= display.Height;
        }
        return screen;
    }

    private Vector2 from_pixel(Vector2 input) {
        var screen = input;
        if(output_mode_type == OutputModeType.absolute && absolute_output_mode != null) {
            var display = absolute_output_mode.Output;
            
            screen.X /= display.Width;
            screen.Y /= display.Height;

            screen *= 2f;
            screen.X -= 1f;
            screen.Y -= 1f;
        }
        return from_unit_screen(screen);
    }

    public event Action<IDeviceReport>? Emit;

    public void Consume(IDeviceReport value) {
        if (value is ITabletReport report) {
            report.Position = filter(report.Position);
        }

        Emit?.Invoke(value);
    }

    private float distance(float a, float b) {
        return Math.Abs(a - b);
    }

    public Vector2 filter(Vector2 input) {
        if(info != null) {
            Vector2 pos = to_pixel(input);
            var display = absolute_output_mode.Output;
            Vector2 cellSize = new Vector2(display.Width/info.cols, display.Height/info.rows);
            Vector2 cellCenter = cellSize / 2f;

            Vector2 mov = Vector2.Zero;
            Vector2 center = Vector2.Zero;

            var final = Vector2.Zero;
            float factorSumX = 0f;
            float factorSumY = 0f;

            for(var row = 0; row < info.rows; row++) {
                for(var col = 0; col < info.cols; col++) {
                    mov.X = col * cellSize.X;
                    mov.Y = row * cellSize.Y;
                    center = cellCenter + mov;

                    float dist = Vector2.Distance(center, pos);
                    float xFactor = Math.Max(Math.Min((cellSize.X - dist)/cellSize.X, 1f), 0f);
                    float yFactor = Math.Max(Math.Min((cellSize.Y - dist)/cellSize.Y, 1f), 0f);

                    factorSumX += xFactor;
                    factorSumY += yFactor;
                    
                    final.X -= info.offsets[col + (row * info.cols)][0] * xFactor;
                    final.Y -= info.offsets[col + (row * info.cols)][1] * yFactor;
                }
            }

            // Normalize
            final.X /= factorSumX;
            final.Y /= factorSumY;

            final -= absolute_output_mode.Output.Position - (new Vector2(absolute_output_mode.Output.Width, absolute_output_mode.Output.Height) / 2f);
            input -= from_pixel(final);
        }
        return input;
    }

    public PipelinePosition Position => PipelinePosition.PostTransform;
}

public class OffsetInfo {
    public int cols { get; set; }
    public int rows { get; set; }
    public float[][] offsets { get; set; }
}

enum OutputModeType {
    absolute,
    relative,
    unknown
}

