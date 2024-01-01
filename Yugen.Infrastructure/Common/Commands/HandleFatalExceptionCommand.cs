using System;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Infrastructure.Common.Commands
{
  public class HandleFatalExceptionCommand : Command
  {
    public Exception Exception { get; }

    public HandleFatalExceptionCommand(Exception exception)
    {
      Exception = exception;
    }
  }
}
