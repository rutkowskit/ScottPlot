﻿/* Minimal case scatter plot for testing only
 * 
 * !! Avoid temptation to use generics or generic math at this early stage of development
 * 
 */

using ScottPlot.Axis;
using SkiaSharp;

namespace ScottPlot.Plottables;

public class Scatter : IPlottable
{
    public string? Label { get; set; }
    public bool IsVisible { get; set; } = true;
    public IAxes Axes { get; set; } = Axis.Axes.Default;

    public AxisLimits GetAxisLimits() => Data.GetLimits();
    public IEnumerable<LegendItem> LegendItems => EnumerableHelpers.One<LegendItem>(
        new LegendItem
        {
            Label = Label,
            Marker = new(Style.MarkerShape.Circle, Color, MarkerSize),
            Line = new(Color, LineWidth),
        });

    public readonly DataSource.IScatterSource Data;
    public Color Color = new(0, 0, 255);
    public float LineWidth = 1;
    public float MarkerSize = 5;

    public Scatter(DataSource.IScatterSource data)
    {
        Data = data;
    }

    public void Render(SKSurface surface)
    {
        IEnumerable<Pixel> pixels = Data.GetScatterPoints().Select(x => Axes.GetPixel(x));

        using SKPaint paint = new()
        {
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            Color = Color.ToSKColor(),
            StrokeWidth = LineWidth,
        };

        // draw lines
        using SKPath path = new();
        path.MoveTo(pixels.First().X, pixels.First().Y);
        foreach (Pixel pixel in pixels)
        {
            path.LineTo(pixel.X, pixel.Y);
        }
        surface.Canvas.DrawPath(path, paint);

        // draw markers
        paint.IsStroke = false;
        foreach (Pixel pixel in pixels)
        {
            surface.Canvas.DrawCircle(pixel.X, pixel.Y, MarkerSize / 2, paint);
        }
    }
}
