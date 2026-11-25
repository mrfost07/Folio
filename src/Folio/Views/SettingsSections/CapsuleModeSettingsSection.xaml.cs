using Folio.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Automation;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Media;
using Folio.Services;

namespace Folio.Views.SettingsSections;

public sealed partial class CapsuleModeSettingsSection : UserControl
{
    private const string PlacementKey = "Settings.Capsule.PlacementTemplates";
    private static readonly (string Action, string Label)[] PlacementChoices =
    [
        ("personal", "Personal"),
        ("left", "Left"),
        ("right", "Right"),
        ("top", "Top"),
        ("bottom", "Bottom"),
        ("floating", "Floating")
    ];

    private readonly PlacementTemplatePreview _personalPreview = new("personal");
    private bool _placementTemplateBusy;
    private sealed record PlacementChoice(string Action, FontIcon Check);

    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(
            nameof(ViewModel),
            typeof(SettingsViewModel),
            typeof(CapsuleModeSettingsSection),
            new PropertyMetadata(null));

    public CapsuleModeSettingsSection()
    {
        InitializeComponent();
        PopulatePlacementChoices();
        Loaded += PlacementTemplates_Loaded;
    }

    public SettingsViewModel? ViewModel
    {
        get => (SettingsViewModel?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    public event EventHandler<SettingsSectionNavigationRequestedEventArgs>? NavigationRequested;

    private void PopulatePlacementChoices()
    {
        foreach (var (action, label) in PlacementChoices)
        {
            PlacementTemplatePreview preview = action == "personal" ? _personalPreview : new(action);
            var title = new TextBlock
            {
                TextAlignment = TextAlignment.Center,
                TextWrapping = TextWrapping.Wrap
            };
            Localized.SetKey(title, $"{PlacementKey}.{label}");
            var check = new FontIcon
            {
                Glyph = "\uE73E",
                FontSize = 13,
                Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 99, 180, 242)),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Center,
                Visibility = Visibility.Collapsed
            };
            AutomationProperties.SetAccessibilityView(check, AccessibilityView.Raw);
            var labelRow = new Grid();
            title.Margin = new Thickness(16, 0, 16, 0);
            labelRow.Children.Add(title);
            labelRow.Children.Add(check);
            var content = new StackPanel { Spacing = 7 };
            content.Children.Add(preview.Element);
            content.Children.Add(labelRow);
            var item = new GridViewItem
            {
                Tag = new PlacementChoice(action, check),
                Width = 138,
                Content = content
            };
            Localized.SetAutomationNameKey(item, $"{PlacementKey}.{label}");
            PlacementTemplatePicker.Items.Add(item);
        }

        PlacementTemplatePicker.SelectedIndex = 0;
    }

    private async void PlacementTemplates_Loaded(object sender, RoutedEventArgs e) =>
        await RefreshPersonalPreviewAsync();

    private async Task RefreshPersonalPreviewAsync()
    {
        var template = await WidgetPlacementTemplateStore.ReadPreviewAsync();
        _personalPreview.DrawPersonal(template);
    }

    private void PlacementTemplatePicker_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        double width = e.NewSize.Width;
        if (width <= 0)
        {
            return;
        }

        const double cardSpacing = 8;
        const double comfortableCardWidth = 146;
        // Leave room for the GridView viewport inset so the last card in
        // each row fits even when the measured control includes that inset.
        double viewportWidth = Math.Max(1, width - 24);
        int columns = Math.Clamp((int)(viewportWidth / (comfortableCardWidth + cardSpacing)), 1, 3);
        double itemWidth = Math.Max(1, Math.Floor(viewportWidth / columns) - cardSpacing);
        foreach (GridViewItem item in PlacementTemplatePicker.Items)
        {
            item.Width = itemWidth;
        }
    }

    private void PlacementTemplatePicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        foreach (GridViewItem item in PlacementTemplatePicker.Items)
        {
            if (item.Tag is PlacementChoice choice)
            {
                choice.Check.Visibility = item.IsSelected ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        PlacementTemplateStatus.Visibility = Visibility.Collapsed;
    }

    private async void PlacementTemplate_Click(object sender, RoutedEventArgs e)
    {
        if (_placementTemplateBusy || ViewModel is not { } model || sender is not Button button)
        {
            return;
        }

        var choice = (PlacementTemplatePicker.SelectedItem as GridViewItem)?.Tag as PlacementChoice;
        string? action = button.Tag as string ?? choice?.Action;
        if (action is null)
        {
            return;
        }

        _placementTemplateBusy = true;
        SavePlacementTemplateButton.IsEnabled = false;
        ApplyPlacementTemplateButton.IsEnabled = false;
        PlacementTemplatePicker.IsEnabled = false;
        try
        {
            bool success = await model.RunPlacementTemplateActionAsync(action);
            if (success && action == "save")
            {
                await RefreshPersonalPreviewAsync();
                PlacementTemplatePicker.SelectedIndex = 0;
            }

            ShowPlacementStatus(success ? action == "save" ? "Saved" : "Applied" : "Unavailable");
        }
        catch (Exception error)
        {
            App.Log($"[PlacementTemplate] Settings action failed: {error.Message}");
            ShowPlacementStatus("Unavailable");
        }
        finally
        {
            SavePlacementTemplateButton.IsEnabled = true;
            ApplyPlacementTemplateButton.IsEnabled = true;
            PlacementTemplatePicker.IsEnabled = true;
            _placementTemplateBusy = false;
        }
    }

    private void ShowPlacementStatus(string state)
    {
        Localized.SetKey(PlacementTemplateStatus, $"{PlacementKey}.{state}");
        PlacementTemplateStatus.Visibility = Visibility.Visible;
    }

    private async void CollapseAll_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel is { } model)
        {
            await model.SetAllWidgetsCollapsedAsync(true);
        }
    }

    private async void ExpandAll_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel is { } model)
        {
            await model.SetAllWidgetsCollapsedAsync(false);
        }
    }

    private void NestedSettingsButton_Click(object sender, RoutedEventArgs e)
    {
        // Keep this guard aligned with the shell's FrameworkElement match so
        // toolkit SettingsCard entries (ButtonBase, not Button) navigate too.
        if (sender is FrameworkElement { Tag: string sectionTag })
        {
            NavigationRequested?.Invoke(this, new SettingsSectionNavigationRequestedEventArgs(sectionTag));
        }
    }
}
