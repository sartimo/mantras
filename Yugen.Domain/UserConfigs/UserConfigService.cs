using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Yugen.Domain.Common;
using Yugen.Domain.Monitors;
using Yugen.Domain.Windows;

namespace Yugen.Domain.UserConfigs
{
  public class UserConfigService
  {
    /// <summary>
    /// The deserialized user config. Sections of the config can be accessed via getters.
    /// </summary>
    public UserConfig UserConfig { private get; set; }

    public GapsConfig GapsConfig => UserConfig.Gaps;
    public FocusBordersConfig FocusBorderConfig => UserConfig.FocusBorders;
    public GeneralConfig GeneralConfig => UserConfig.General;
    public List<BarConfig> BarConfigs => UserConfig.BarConfigs;
    public List<WorkspaceConfig> WorkspaceConfigs => UserConfig.Workspaces;
    public List<WindowRuleConfig> WindowRules => UserConfig.WindowRules;
    public List<KeybindingConfig> Keybindings => UserConfig.Keybindings;
    public List<BindingMode> BindingModes => UserConfig.BindingModes;

    /// <summary>
    /// Path to the user's config file.
    /// </summary>
    public string UserConfigPath =>
      _startupOptions.ConfigPath ?? _defaultUserConfigPath;

    private readonly string _defaultUserConfigPath = Path.Combine(
      Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
      "./.yugen/config.yaml"
    );

    private readonly WmStartupOptions _startupOptions;

    public UserConfigService(WmStartupOptions startupOptions)
    {
      _startupOptions = startupOptions;
    }

    public readonly List<WindowRuleConfig> DefaultWindowRules = GetDefaultWindowRules();

    private static List<WindowRuleConfig> GetDefaultWindowRules()
    {
      var windowRules = new List<WindowRuleConfig>();

      var classNamesToFloat = new List<string> {
        // Windows 10 dialog shown when moving and deleting files.
        "OperationStatusWindow",
      };

      foreach (var className in classNamesToFloat)
      {
        var windowRule = new WindowRuleConfig()
        {
          MatchClassName = className,
          Command = "set floating",
        };

        windowRules.Add(windowRule);
      }

      var chromiumBrowserProcessNames = new List<string> {
        "chrome",
        "msedge",
        "opera",
        "vivaldi",
        "brave",
      };

      // Electron apps do not have invisible borders and are thus over-corrected by the default
      // border fix. To match these apps, get windows with the class name 'Chrome_WidgetWin_1' that
      // are not Chromium-based browsers (since the browser windows do have invisble borders).
      var resizeElectronBorderWindowRule = new WindowRuleConfig()
      {
        MatchProcessName = $"/^(?!({string.Join("|", chromiumBrowserProcessNames)})$)/",
        MatchClassName = "/Chrome_WidgetWin_*/",
        Command = "resize borders 0px -7px -7px -7px",
      };

      windowRules.Add(resizeElectronBorderWindowRule);

      var processNamesToIgnore = new List<string> {
        "SearchApp",
        "SearchHost",
        "ShellExperienceHost",
        "StartMenuExperienceHost",
      };

      foreach (var processName in processNamesToIgnore)
      {
        var windowRule = new WindowRuleConfig()
        {
          MatchProcessName = processName,
          Command = "ignore",
        };

        windowRules.Add(windowRule);
      }

      return windowRules;
    }

    public WorkspaceConfig GetWorkspaceConfigByName(string workspaceName)
    {
      return WorkspaceConfigs.Find(
        (workspaceConfig) => workspaceConfig.Name == workspaceName
      );
    }

    public BindingMode GetBindingModeByName(string bindingModeName)
    {
      return BindingModes.Find(
        (bindingMode) => bindingMode.Name == bindingModeName
      );
    }

    public BarConfig GetBarConfigForMonitor(Monitor monitor)
    {
      var boundMonitor = BarConfigs
        .OfType<MultiBarConfig>()
        .FirstOrDefault(config => config.BindToMonitor == monitor.DeviceName);

      return boundMonitor ?? BarConfigs[0];
    }

    public List<WindowRuleConfig> GetMatchingWindowRules(Window window)
    {
      return WindowRules.Where(rule =>
      {
        return rule.ProcessNameRegex?.IsMatch(window.ProcessName) != false &&
          rule.ClassNameRegex?.IsMatch(window.ClassName) != false &&
          rule.TitleRegex?.IsMatch(window.Title) != false;
      }).ToList();
    }
  }
}
