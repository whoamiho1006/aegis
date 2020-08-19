using System;
using System.Collections.Generic;
using System.Text;

namespace Aegis.Endpoints.HTTP
{
    internal class Listener : IListener
    {
        public IRequest Accept(int Timeout)
        {
            throw new NotImplementedException();
        }

        public bool HasPending()
        {
            throw new NotImplementedException();
        }

        public bool TryAccept(int Timeout, out IRequest Request)
        {
            throw new NotImplementedException();
        }
    }
}
