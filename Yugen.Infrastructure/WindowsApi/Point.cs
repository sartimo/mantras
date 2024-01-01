using System.Runtime.InteropServices;

namespace Yugen.Infrastructure.WindowsApi
{
  [StructLayout(LayoutKind.Sequential)]
  public struct Point
  {
    public int X;
    public int Y;
  }
}
