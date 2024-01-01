using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Windows.Threading;
using Yugen.Bar.Common;
using Yugen.Bar.Components;
using Yugen.Domain.Monitors;
using Yugen.Domain.UserConfigs;
using Yugen.Infrastructure.Utils;

namespace Yugen.Bar
{
  public class BarViewModel : ViewModelBase
  {
    public readonly Subject<bool> WindowClosing = new();

    public Monitor Monitor { get; }
    public Dispatcher Dispatcher { get; }
    public BarConfig BarConfig { get; }

    public BarPosition Position => BarConfig.Position;
    public bool AlwaysOnTop => BarConfig.AlwaysOnTop;
    public string Background => XamlHelper.FormatColor(BarConfig.Background);
    public string Foreground => XamlHelper.FormatColor(BarConfig.Foreground);
    public string FontFamily => BarConfig.FontFamily;
    public string FontWeight => BarConfig.FontWeight;
    public string FontSize => XamlHelper.FormatSize(BarConfig.FontSize);
    public string BorderColor => XamlHelper.FormatColor(BarConfig.BorderColor);
    public string BorderWidth => XamlHelper.FormatRectShorthand(BarConfig.BorderWidth);
    public string BorderRadius => XamlHelper.FormatSize(BarConfig.BorderRadius);
    public string Padding => XamlHelper.FormatRectShorthand(BarConfig.Padding);
    public double Opacity => BarConfig.Opacity;

    private TextComponentViewModel _componentSeparatorLeft => new(
        this, new TextComponentConfig
        {
          Text = BarConfig.ComponentSeparator.LabelLeft
            ?? BarConfig.ComponentSeparator.Label
        }
    );

    private TextComponentViewModel _componentSeparatorCenter => new(
        this, new TextComponentConfig
        {
          Text = BarConfig.ComponentSeparator.LabelCenter
            ?? BarConfig.ComponentSeparator.Label
        }
    );

    private TextComponentViewModel _componentSeparatorRight => new(
        this, new TextComponentConfig
        {
          Text = BarConfig.ComponentSeparator.LabelRight
            ?? BarConfig.ComponentSeparator.Label
        }
    );

    public List<ComponentViewModel> ComponentsLeft =>
      InsertComponentSeparator(
        CreateComponentViewModels(BarConfig.ComponentsLeft),
          _componentSeparatorLeft);

    public List<ComponentViewModel> ComponentsCenter =>
      InsertComponentSeparator(
        CreateComponentViewModels(BarConfig.ComponentsCenter),
          _componentSeparatorCenter);

    public List<ComponentViewModel> ComponentsRight =>
      InsertComponentSeparator(
        CreateComponentViewModels(BarConfig.ComponentsRight),
          _componentSeparatorRight);

    private static List<ComponentViewModel> InsertComponentSeparator(
      List<ComponentViewModel> componentViewModels, TextComponentViewModel componentSeparator
    )
    {
      if (!string.IsNullOrEmpty(componentSeparator.Text))
        componentViewModels.Intersperse(componentSeparator);

      return componentViewModels;
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        WindowClosing.Dispose();
      }

      base.Dispose(disposing);
    }

    private List<ComponentViewModel> CreateComponentViewModels(
      List<BarComponentConfig> componentConfigs)
    {
      return componentConfigs.ConvertAll<ComponentViewModel>(config => config switch
      {
        BatteryComponentConfig bsc => new BatteryComponentViewModel(this, bsc),
        BindingModeComponentConfig bmc => new BindingModeComponentViewModel(this, bmc),
        ClockComponentConfig ccc => new ClockComponentViewModel(this, ccc),
        TextComponentConfig tcc => new TextComponentViewModel(this, tcc),
        WeatherComponentConfig wcc => new WeatherComponentViewModel(this, wcc),
        NetworkComponentConfig ncc => new NetworkComponentViewModel(this, ncc),
        TilingDirectionComponentConfig tdc => new TilingDirectionComponentViewModel(this, tdc),
        WorkspacesComponentConfig wcc => new WorkspacesComponentViewModel(this, wcc),
        WindowTitleComponentConfig wtcc => new WindowTitleComponentViewModel(this, wtcc),
        VolumeComponentConfig vcc => new VolumeComponentViewModel(this, vcc),
        ImageComponentConfig bscc => new ImageComponentViewModel(this, bscc),
        SystemTrayComponentConfig stcc => new SystemTrayComponentViewModel(this, stcc),
        CpuComponentConfig cpupc => new CpuComponentViewModel(this, cpupc),
        GpuComponentConfig gpupc => new GpuComponentViewModel(this, gpupc),
        MemoryComponentConfig rampc => new MemoryComponentViewModel(this, rampc),
        TextFileComponentConfig stc => new TextFileComponentViewModel(this, stc),
        MusicComponentConfig mcc => new MusicComponentViewModel(this, mcc),
        _ => throw new ArgumentOutOfRangeException(nameof(config)),
      });
    }

    public BarViewModel(Monitor monitor, Dispatcher dispatcher, BarConfig barConfig)
    {
      Monitor = monitor;
      Dispatcher = dispatcher;
      BarConfig = barConfig;
    }
  }
}
