using System;

namespace Yugen.Infrastructure.Exceptions
{
  public class ExceptionHandlingOptions
  {
    public string ErrorLogPath { get; set; }
    public Func<Exception, string> ErrorLogMessageDelegate { get; set; }
  }
}
