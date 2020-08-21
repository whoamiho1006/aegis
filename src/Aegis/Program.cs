using Aegis.Blockchains.Algorithms;
using Aegis.Blockchains;
using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using Aegis.Workers.Tasks;
using Aegis.Endpoints.HTTP;
using System.Net;

namespace Aegis
{
    class Program
    {
        static void Main(string[] args)
        {
            Engine Engine = new Engine();

            Kernel Kernel = new Kernel(Engine);
            Listener Listener = new Listener(8088);

            Kernel.Map("/", new DirectoryInfo("Assets"));

            Kernel.Use(Listener);
            Engine.Use(Kernel);

            Engine.Start();
        }
    }
}
