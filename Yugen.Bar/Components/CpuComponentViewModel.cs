using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reactive.Linq;
using Yugen.Domain.UserConfigs;
using Yugen.Infrastructure;
using Yugen.Infrastructure.WindowsApi;

namespace Yugen.Bar.Components
{
  public class CpuComponentViewModel : ComponentViewModel
  {
    private readonly CpuComponentConfig _config;
    private readonly CpuStatsService _cpuStatsService;

    private LabelViewModel _label;
    public LabelViewModel Label
    {
      get => _label;
      protected set => SetField(ref _label, value);
    }

    public CpuComponentViewModel(
      BarViewModel parentViewModel,
      CpuComponentConfig config) : base(parentViewModel, config)
    {
      _config = config;
      _cpuStatsService = ServiceLocator.GetRequiredService<CpuStatsService>();

      Observable.Timer(
        TimeSpan.Zero,
        TimeSpan.FromMilliseconds(_config.RefreshIntervalMs)
      )
        .TakeUntil(_parentViewModel.WindowClosing)
        .Subscribe((_) => Label = CreateLabel());
    }

    private LabelViewModel CreateLabel()
    {
      var variableDictionary = new Dictionary<string, Func<string>>()
      {
        {
          "percent_usage",
          () => _cpuStatsService.GetCpuUsage().ToString("0", CultureInfo.InvariantCulture)
        }
      };

      return XamlHelper.ParseLabel(_config.Label, variableDictionary, this);
    }
  }
}
