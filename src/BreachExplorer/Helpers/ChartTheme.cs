using BlazorBootstrap;

namespace BreachExplorer.Helpers;

public static class ChartTheme
{
    private const string LightBlue = "#b8e8ff";
    private const string DarkBlue = "#0a2d52";

    /// <summary>
    /// Light blue = highest value (most data stolen), dark blue = lowest.
    /// </summary>
    public static List<string> GenerateBlueGradientColors(IReadOnlyList<double?> data)
    {
        if (data == null || data.Count == 0)
        {
            return new List<string>();
        }

        var values = data.Where(d => d.HasValue).Select(d => d!.Value).ToList();
        if (values.Count == 0)
        {
            return data.Select(_ => "rgba(120, 217, 255, 0.75)").ToList();
        }

        var min = values.Min();
        var max = values.Max();
        var range = max - min;
        if (range <= 0)
        {
            return data.Select(_ => ToRgba(ParseHex(LightBlue), 0.92)).ToList();
        }

        var colors = new List<string>(data.Count);
        foreach (var value in data)
        {
            if (!value.HasValue)
            {
                colors.Add("rgba(120, 217, 255, 0.5)");
                continue;
            }

            var position = (max - value.Value) / range;
            colors.Add(InterpolateBlue(position));
        }

        return colors;
    }

    public static List<string> GenerateBlueGradientBorders(IReadOnlyList<string> fills)
    {
        return fills.Select(c => c.Replace("0.92)", "1)")).ToList();
    }

    public static int GetChartHeight(int barCount, bool horizontal, int viewportWidth = 1200)
    {
        if (!horizontal)
        {
            return viewportWidth < 576 ? 300 : viewportWidth < 768 ? 340 : 380;
        }

        var perBar = viewportWidth < 576 ? 28 : viewportWidth < 768 ? 32 : 36;
        return Math.Clamp(barCount * perBar + 100, 280, viewportWidth < 576 ? 520 : 640);
    }

    public static void ApplyDarkTheme(BarChartOptions options, bool horizontal)
    {
        options.Responsive = true;
        options.MaintainAspectRatio = false;
        EnsureChartStructure(options);

        // Horizontal bars: nearest + intersect targets the bar under the cursor (index mode misaligns on indexAxis 'y').
        options.Interaction = horizontal
            ? new Interaction { Mode = InteractionMode.Nearest, Intersect = true }
            : new Interaction { Mode = InteractionMode.Index, Intersect = true };

        ConfigureScale(options.Scales.X!, horizontal ? "Records exposed" : "Company", showGrid: horizontal);
        ConfigureScale(options.Scales.Y!, horizontal ? "Breach" : "Records exposed", showGrid: !horizontal);

        options.Plugins.Legend ??= new ChartPluginsLegend();
        options.Plugins.Legend.Display = false;

        options.Plugins.Tooltip ??= new ChartPluginsTooltip();
        options.Plugins.Tooltip.BackgroundColor = "rgba(8, 15, 28, 0.96)";
        options.Plugins.Tooltip.TitleColor = "#78d9ff";
        options.Plugins.Tooltip.BodyColor = "#edf4ff";
    }

    private static void EnsureChartStructure(BarChartOptions options)
    {
        options.Scales ??= new Scales();
        options.Scales.X ??= new ChartAxes();
        options.Scales.Y ??= new ChartAxes();
        options.Plugins ??= new BarChartPlugins();
    }

    public static void SetAxisOrientation(BarChartOptions options, bool horizontal)
    {
        options.IndexAxis = horizontal ? "y" : "x";
        ApplyDarkTheme(options, horizontal);
    }

    private static void ConfigureScale(ChartAxes scale, string title, bool showGrid)
    {
        scale.Title = new ChartAxesTitle
        {
            Text = title,
            Display = !string.IsNullOrEmpty(title),
            Color = "#b8c7df"
        };

        scale.Ticks = new ChartAxesTicks
        {
            Color = "#8ea1be"
        };

        scale.Grid = new ChartAxesGrid
        {
            Display = showGrid,
            Color = "rgba(176, 197, 228, 0.12)",
            LineWidth = 1
        };

        scale.Border = new ChartAxesBorder
        {
            Display = true,
            Color = "rgba(176, 197, 228, 0.22)",
            Width = 1
        };
    }

    private static string InterpolateBlue(double position)
    {
        var light = ParseHex(LightBlue);
        var dark = ParseHex(DarkBlue);
        var t = Math.Clamp(position, 0, 1);

        var r = (int)(light.R + (dark.R - light.R) * t);
        var g = (int)(light.G + (dark.G - light.G) * t);
        var b = (int)(light.B + (dark.B - light.B) * t);

        return $"rgba({r}, {g}, {b}, 0.92)";
    }

    private static (int R, int G, int B) ParseHex(string hex)
    {
        hex = hex.TrimStart('#');
        return (
            Convert.ToInt32(hex[..2], 16),
            Convert.ToInt32(hex.Substring(2, 2), 16),
            Convert.ToInt32(hex.Substring(4, 2), 16)
        );
    }

    private static string ToRgba((int R, int G, int B) rgb, double alpha) =>
        $"rgba({rgb.R}, {rgb.G}, {rgb.B}, {alpha})";
}
