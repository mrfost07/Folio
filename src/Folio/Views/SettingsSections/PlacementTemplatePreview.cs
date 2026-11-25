using Folio.Models;
using Folio.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;
using Windows.UI;

namespace Folio.Views.SettingsSections;

/// <summary>A small diagram of the placement represented by a template card.</summary>
internal sealed class PlacementTemplatePreview
{
    private static readonly Rect PreviewFrame = new(0, 0, 176, 96);
    private static readonly SolidColorBrush ScreenFill = Brush(17, 31, 48);
    private static readonly SolidColorBrush ScreenEdge = Brush(61, 84, 107);
    private static readonly SolidColorBrush WidgetFill = Brush(44, 68, 92);
    private static readonly SolidColorBrush WidgetHeading = Brush(83, 114, 143);
    private static readonly SolidColorBrush CapsuleFill = Brush(102, 188, 247);
    private readonly Canvas _canvas = new() { Width = 176, Height = 96 };

    internal PlacementTemplatePreview(string action)
    {
        Element = new Viewbox
        {
            Child = _canvas,
            Stretch = Stretch.Uniform,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            IsHitTestVisible = false
        };
        AutomationProperties.SetAccessibilityView(Element, AccessibilityView.Raw);
        if (action == "personal")
        {
            DrawPersonal(null);
        }
        else
        {
            DrawPreset(action);
        }
    }

    internal Viewbox Element { get; }

    private void DrawPreset(string action)
    {
        DrawScreen(PreviewFrame);
        bool vertical = action is "left" or "right";
        double x = action switch
        {
            "left" => 8,
            "right" => 142,
            _ => 31
        };
        double y = action switch
        {
            "top" => 8,
            "bottom" => 79,
            "floating" => 44,
            _ => 24
        };

        const int count = 5;
        for (int index = 0; index < count; index++)
        {
            DrawBox(new Rect(
                x + (vertical ? 0 : index * 23),
                y + (vertical ? index * 10 : 0),
                vertical ? 26 : 21,
                vertical ? 7 : 8), CapsuleFill, radius: 2);
        }
    }

    internal void DrawPersonal(WidgetPlacementTemplate? template)
    {
        _canvas.Children.Clear();
        var monitors = template?.Monitors?.Where(m => m.WorkAreaWidth > 0 && m.WorkAreaHeight > 0).ToArray();
        if (template is not null && monitors is { Length: > 0 })
        {
            DrawSavedMonitors(template, monitors);
            return;
        }

        DrawScreen(PreviewFrame);
        if (template is null)
        {
            DrawBox(new Rect(79, 47, 18, 2), WidgetHeading, radius: 1);
            DrawBox(new Rect(87, 39, 2, 18), WidgetHeading, radius: 1);
            return;
        }

        // Templates saved before monitor geometry was included still carry
        // a display-relative placement reference for each widget.
        foreach (WidgetPlacementTemplateSurface surface in template.Surfaces.Values)
        {
            WidgetSurfaceLayoutProfile layout = surface.Placement;
            if (layout.Reference is not { FrameWidth: > 0, FrameHeight: > 0 } reference)
            {
                continue;
            }

            double width = surface.IsCollapsed ? layout.CompactWidth ?? Math.Min(220, reference.Width) : reference.Width;
            double height = surface.IsCollapsed ? 36 : reference.Height;
            string? anchor = surface.IsCollapsed
                ? reference.CompactPositionAnchor ?? reference.PositionAnchor
                : reference.PositionAnchor;
            double marginX = surface.IsCollapsed ? reference.CompactPositionMarginX ?? reference.PositionMarginX : reference.PositionMarginX;
            double marginY = surface.IsCollapsed ? reference.CompactPositionMarginY ?? reference.PositionMarginY : reference.PositionMarginY;
            double x = anchor is "RightTop" or "RightBottom" ? reference.FrameWidth - width - marginX : marginX;
            double y = anchor is "LeftBottom" or "RightBottom" ? reference.FrameHeight - height - marginY : marginY;
            DrawSurface(PreviewFrame, x / reference.FrameWidth, y / reference.FrameHeight,
                width / reference.FrameWidth, height / reference.FrameHeight, surface.IsCollapsed);
        }
    }

    private void DrawSavedMonitors(
        WidgetPlacementTemplate template,
        WidgetTopologyMonitorProfile[] monitors)
    {
        double left = monitors.Min(m => m.WorkAreaX);
        double top = monitors.Min(m => m.WorkAreaY);
        double width = monitors.Max(m => (double)m.WorkAreaX + m.WorkAreaWidth) - left;
        double height = monitors.Max(m => (double)m.WorkAreaY + m.WorkAreaHeight) - top;
        double factor = Math.Min(PreviewFrame.Width / width, PreviewFrame.Height / height);
        var frames = monitors.ToDictionary(m => m, m => new Rect(
            (PreviewFrame.Width - width * factor) / 2 + (m.WorkAreaX - left) * factor,
            (PreviewFrame.Height - height * factor) / 2 + (m.WorkAreaY - top) * factor,
            m.WorkAreaWidth * factor, m.WorkAreaHeight * factor));
        foreach (Rect frame in frames.Values)
        {
            DrawScreen(frame);
        }

        foreach (WidgetPlacementTemplateSurface surface in template.Surfaces.Values)
        {
            WidgetSurfaceLayoutProfile layout = surface.Placement;
            WidgetCompactPlacement? compact = surface.IsCollapsed ? layout.CompactPlacement : null;
            double x = compact?.X ?? layout.X;
            double y = compact?.Y ?? layout.Y;
            string? stableId = compact?.PositionMonitorStableId ?? layout.PositionMonitorStableId;
            WidgetTopologyMonitorProfile monitor = monitors.FirstOrDefault(m =>
                !string.IsNullOrEmpty(stableId) && string.Equals(m.StableId, stableId, StringComparison.OrdinalIgnoreCase))
                ?? monitors.FirstOrDefault(m =>
                    x >= m.WorkAreaX && x < (double)m.WorkAreaX + m.WorkAreaWidth &&
                    y >= m.WorkAreaY && y < (double)m.WorkAreaY + m.WorkAreaHeight)
                ?? monitors[0];
            double scale = monitor.DpiScale > 0 ? monitor.DpiScale : 1;
            double widgetWidth = (surface.IsCollapsed ? layout.CompactWidth ?? Math.Min(220, layout.Width) : layout.Width) * scale;
            double widgetHeight = (surface.IsCollapsed ? 36 : layout.Height) * scale;
            DrawSurface(frames[monitor], (x - monitor.WorkAreaX) / monitor.WorkAreaWidth,
                (y - monitor.WorkAreaY) / monitor.WorkAreaHeight,
                widgetWidth / monitor.WorkAreaWidth, widgetHeight / monitor.WorkAreaHeight, surface.IsCollapsed);
        }
    }

    private void DrawScreen(Rect frame)
    {
        DrawBox(frame, ScreenFill, radius: 6, stroke: ScreenEdge);
        DrawBox(new Rect(frame.X + frame.Width * 0.44, frame.Y + frame.Height - 4,
            frame.Width * 0.12, 1), ScreenEdge, radius: 0.5);
    }

    private void DrawSurface(Rect frame, double x, double y, double width, double height, bool collapsed)
    {
        if (frame.Width < 7 || frame.Height < 7 || !double.IsFinite(x) || !double.IsFinite(y) ||
            !double.IsFinite(width) || !double.IsFinite(height))
        {
            return;
        }

        double safeWidth = Math.Clamp(width * frame.Width, 3, frame.Width - 4);
        double safeHeight = Math.Clamp(height * frame.Height, 3, frame.Height - 4);
        double safeX = frame.X + Math.Clamp(x * frame.Width, 2, frame.Width - safeWidth - 2);
        double safeY = frame.Y + Math.Clamp(y * frame.Height, 2, frame.Height - safeHeight - 2);
        DrawBox(new Rect(safeX, safeY, safeWidth, safeHeight), collapsed ? CapsuleFill : WidgetFill, radius: 2);
        if (!collapsed && safeWidth >= 9 && safeHeight >= 8)
        {
            DrawBox(new Rect(safeX + 2, safeY + 2, safeWidth - 4, 1), WidgetHeading, radius: 0.5);
        }
    }

    private void DrawBox(Rect rectangle, Brush fill, double radius, Brush? stroke = null)
    {
        var box = new Border
        {
            Width = rectangle.Width,
            Height = rectangle.Height,
            Background = fill,
            CornerRadius = new CornerRadius(radius),
            BorderBrush = stroke,
            BorderThickness = new Thickness(stroke is null ? 0 : 1)
        };
        Canvas.SetLeft(box, rectangle.X);
        Canvas.SetTop(box, rectangle.Y);
        _canvas.Children.Add(box);
    }

    private static SolidColorBrush Brush(byte red, byte green, byte blue) =>
        new(Color.FromArgb(255, red, green, blue));
}
