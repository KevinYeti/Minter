using System;

namespace Minter.Interface
{
    public interface ILogger
    {
        void Write(string log);

        string[] Read(DateTime date);

        int Count(DateTime date);

    }
}
