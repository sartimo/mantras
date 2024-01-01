using System.Collections.Generic;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Common.Commands
{
  public class ExecProcessCommand : Command
  {
    public string ProcessName { get; }
    public List<string> Args { get; }

    public ExecProcessCommand(string processName, List<string> args)
    {
      ProcessName = processName;
      Args = args;
    }
  }
}
