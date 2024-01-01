using System;
using Yugen.Domain.Containers;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Windows.Commands
{
  public class ManageWindowCommand : Command
  {
    public IntPtr WindowHandle { get; }
    public SplitContainer TargetParent { get; }

    public ManageWindowCommand(
      IntPtr windowHandle,
      SplitContainer targetParent = null)
    {
      WindowHandle = windowHandle;
      TargetParent = targetParent;
    }
  }
}
