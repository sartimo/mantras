using System.Collections.Generic;
using Yugen.Bar.Common;

namespace Yugen.Bar.Components
{
  public class LabelViewModel : ViewModelBase
  {
    public List<LabelSpan> Spans { get; }

    public LabelViewModel(List<LabelSpan> spans)
    {
      Spans = spans;
    }
  }
}
