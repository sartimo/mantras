using System.ComponentModel.DataAnnotations;

namespace Yugen.Domain.UserConfigs
{
  public class BarComponentConfig
  {
    [Required]
    public string Type { get; set; }

    public string Margin { get; set; } = "0";

    /// <summary>
    /// Background of the bar component. Use transparent as a default, since falling back
    /// to bar background config looks weird with nested semi-transparent backgrounds.
    /// </summary>
    public string Background { get; set; } = "transparent";

    /// <summary>
    /// Fallback to bar foreground config if unset.
    /// </summary>
    public string Foreground { get; set; }

    /// <summary>
    /// Fallback to bar font family config if unset.
    /// </summary>
    public string FontFamily { get; set; }

    /// <summary>
    /// Fallback to bar font weight config if unset.
    /// </summary>
    public string FontWeight { get; set; }

    /// <summary>
    /// Fallback to bar font size config if unset.
    /// </summary>
    public string FontSize { get; set; }

    public string BorderColor { get; set; } = "blue";

    /// <summary>
    /// Radius of the border in pixels.
    /// </summary>
    public string BorderRadius { get; set; } = "0px";

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
  }
}
