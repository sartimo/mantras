namespace Yugen.Infrastructure.Bussing
{
  public class Command
  {
    public string Name => GetType().Name;
  }
}
