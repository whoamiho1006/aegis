using Aegis.Endpoints.HTTP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Aegis.Endpoints.Handlers
{
    public class Authorizer : IHandler
    {
        public Context Handle(Context Context)
        {
            MemoryStream MemStream = new MemoryStream();

            Context.Response.StatusCode = EStatusCode.Okay;
            Context.Response.Output = new ResponseContent
            {
                MimeType = "application/json",
                Content = MemStream,
                Close = X => X.Dispose(),
                Encoding = Encoding.UTF8,
                Length = 0,
                Offset = 0
            };

            /*
             
            Required header:
                Authorization: aegis PUB-KEY; seq=n; sig=signature
            
            SEQ can't less or equaler than before.
             */

            using (StreamWriter Writer = new StreamWriter(
                MemStream, Encoding.UTF8, 1024, true))
            {
                Writer.Write("{ 'pvt': \"");
                Writer.Write("\", 'pub': \"");

                Writer.Write("\" }");

                Writer.Flush();
            }

            MemStream.Seek(0, SeekOrigin.Begin);
            return Context;
        }
    }
}
