using System;
using System.Collections.Generic;

namespace Yugen.Infrastructure.WindowsApi
{
  public class SystemTrayIconConfig
  {
    public string HoverText { get; init; }
    public string IconResourceName { get; init; }
    public Dictionary<string, Action> Actions { get; init; }
  }
}
