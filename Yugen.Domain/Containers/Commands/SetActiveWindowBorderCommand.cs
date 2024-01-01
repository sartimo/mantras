using Yugen.Domain.Windows;
using Yugen.Infrastructure.Bussing;
using Yugen.Infrastructure.WindowsApi;

namespace Yugen.Domain.Containers.Commands
{
  public class SetActiveWindowBorderCommand : Command
  {
    public Window TargetWindow { get; }

    /// <summary>
    /// Sets the newly focused window's border and removes border on older window.  
    /// </summary>
    public SetActiveWindowBorderCommand(Window target)
    {
      TargetWindow = target;
    }
  }
}
