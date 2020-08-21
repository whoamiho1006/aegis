using Aegis.Blockchains.Algorithms;
using Aegis.Endpoints.HTTP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Aegis.Endpoints.Handlers
{
    public class KeyPairGenerator : IHandler
    {
        public Context Handle(Context Context)
        {
            byte[] Pvt = SECP256K1.Instance.NewPrivateKey();
            byte[] Pub = SECP256K1.Instance.ToPublicKey(Pvt);

            MemoryStream MemStream = new MemoryStream();

            Context.Response.StatusCode = EStatusCode.Okay;
            Context.Response.Output = new ResponseContent
            {
                MimeType = "application/json",
                Content = MemStream,
                Close = X => X.Dispose(),
                Encoding = Encoding.UTF8,
                Length = 0, Offset = 0
            };

            using (StreamWriter Writer = new StreamWriter(
                MemStream, Encoding.UTF8, 1024, true))
            {
                Writer.Write("{ \"pvt\": \"");
                Writer.Write(Convert.ToBase64String(Pvt));

                Writer.Write("\", \"pub\": \"");
                Writer.Write(Convert.ToBase64String(Pub));

                Writer.Write("\" }");

                Writer.Flush();
            }

            MemStream.Seek(0, SeekOrigin.Begin);
            return Context;
        }
    }
}
