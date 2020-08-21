using Aegis.Endpoints.Handlers;
using Aegis.Endpoints.HTTP;

namespace Aegis
{
    class Program
    {
        static void Main(string[] args)
        {
            Engine Engine = new Engine();

            Kernel Kernel = new Kernel(Engine);
            Listener Listener = new Listener(8088);

            Kernel.Map("/aegis/generate", new KeyPairGenerator());
            Kernel.Map("/aegis/authorize", new Authorizer());

            Kernel.Use(Listener);
            Engine.Use(Kernel);

            Engine.Start();
        }
    }
}
