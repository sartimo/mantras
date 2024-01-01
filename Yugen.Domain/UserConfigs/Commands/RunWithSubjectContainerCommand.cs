using System.Collections.Generic;
using Yugen.Domain.Containers;
using Yugen.Infrastructure.Bussing;

namespace Yugen.Domain.UserConfigs.Commands
{
  public class RunWithSubjectContainerCommand : Command
  {
    public IEnumerable<string> CommandStrings { get; }
    public Container SubjectContainer { get; }

    public RunWithSubjectContainerCommand(
      IEnumerable<string> commandStrings,
      Container subjectContainer)
    {
      CommandStrings = commandStrings;
      SubjectContainer = subjectContainer;
    }

    public RunWithSubjectContainerCommand(IEnumerable<string> commandStrings)
    {
      CommandStrings = commandStrings;
    }
  }
}
