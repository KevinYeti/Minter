using Minter.Monitor.ComponentProvider;

namespace Minter.Monitor.ComponentProvider
{
    public class ComponentConfig
    {
        public static void Register()
        {
            QuartzProvider.Init();
            
        }
    }
}
