using Autofac;
using Minter.FileLog;
using Minter.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minter.Quartz
{
    public static class Global
    {
        static Global()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<FileLogger>().As<ILogger>();
            Container = builder.Build();

            //var logger = Global.Container.Resolve<ILogger>();
            //logger.Write("autofac init completed: ILogger");
        }
        public static IContainer Container { get; set; }
    }
}
