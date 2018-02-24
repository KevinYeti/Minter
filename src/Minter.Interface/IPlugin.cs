using Quartz;

namespace Minter.Interface
{
    public interface IPlugin : IJob
    {
        string Name { get; }
        string Group { get;  }

        IJobDetail Job { get; }
    }
}
