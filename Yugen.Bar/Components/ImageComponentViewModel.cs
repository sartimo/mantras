using Yugen.Domain.UserConfigs;

namespace Yugen.Bar.Components
{
  public class ImageComponentViewModel : ComponentViewModel
  {
    private ImageComponentConfig _config => _componentConfig as ImageComponentConfig;

    public string Source => _config.Source;

    public ImageComponentViewModel(
      BarViewModel parentViewModel,
      ImageComponentConfig config) : base(parentViewModel, config)
    {
    }
  }
}
