using Windows.Media.Control;

namespace Yugen.Infrastructure.WindowsApi
{
  public class CurrentMediaPlaybackChangedEventArgs
  {
    public GlobalSystemMediaTransportControlsSessionPlaybackStatus PlaybackState { get; set; }
  }
}
