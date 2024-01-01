using System;
using System.Diagnostics;
using System.Windows.Forms;
using Yugen.Domain.Common.Commands;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.Common.CommandHandlers
{
  public class ExecProcessHandler : ICommandHandler<ExecProcessCommand>
  {
    public CommandResponse Handle(ExecProcessCommand command)
    {
      var processName = command.ProcessName;
      var args = command.Args;

      try
      {
        using var process = new Process();
        process.StartInfo = new ProcessStartInfo
        {
          // Expand env variables in the process name (eg. "%ProgramFiles%").
          FileName = Environment.ExpandEnvironmentVariables(processName),
          Arguments = string.Join(" ", args),
          UseShellExecute = true,
          // Set user profile directory as the working dir. This affects the starting directory
          // of terminal processes (eg. CMD, Git bash, etc).
          WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
        };
        process.Start();
      }
      catch (Exception exception)
      {
        // Alert the user of the error.
        // TODO: Link to documentation for `exec` command (no proper documentation yet).
        MessageBox.Show(exception.Message);
      }

      return CommandResponse.Ok;
    }
  }
}
