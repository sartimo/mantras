using System.Collections.Generic;

namespace Yugen.Domain.UserConfigs
{
  public class BarConfig
  {
    public bool Enabled { get; set; } = true;

    public bool AlwaysOnTop { get; set; } = false;

    public string OffsetX { get; set; } = "0px";

    public string OffsetY { get; set; } = "0px";

    public string BorderRadius { get; set; } = "0px";

    public string Height { get; set; } = "30px";

    public BarPosition Position { get; set; } = BarPosition.Top;

    public double Opacity { get; set; } = 1.0;

    public string Background { get; set; } = "black";

    public string Foreground { get; set; } = "white";

    public string FontFamily { get; set; } = "Segoe UI";

    public string FontWeight { get; set; } = "Normal";

    public string FontSize { get; set; } = "13";

    public string BorderColor { get; set; } = "blue";

    public BarComponentSeparatorConfig ComponentSeparator { get; set; } = new();

    /// <summary>
    /// Width of the border in pixels. To set a different border width for each side, specify four
    /// values (eg. "5 0 5 0"). The borders widths apply to the top, right, bottom, and left in that
    /// order.
    /// </summary>
    public string BorderWidth { get; set; } = "0";

    /// <summary>
    /// Padding in pixels.
    /// </summary>
    public string Padding { get; set; } = "0";

    public List<BarComponentConfig> ComponentsLeft { get; set; } = new List<BarComponentConfig>();

    public List<BarComponentConfig> ComponentsCenter { get; set; } = new List<BarComponentConfig>();

    public List<BarComponentConfig> ComponentsRight { get; set; } = new List<BarComponentConfig>();
  }
}
