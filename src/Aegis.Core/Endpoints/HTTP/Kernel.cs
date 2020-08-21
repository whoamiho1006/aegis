using Aegis.Endpoints.Common;
using Aegis.Utilities;
using Aegis.Workers;
using Aegis.Workers.Tasks;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Aegis.Endpoints.HTTP
{
    /// <summary>
    /// Kernel for handling HTTP requests and generating responses.
    /// </summary>
    public sealed class Kernel : IKernel
    {
        private List<IListener> m_Listener;
        internal List<IMiddleware> m_Middleware;

        private Thread m_Thread;
        private ManualResetEvent m_Event;

        private ConcurrentDictionary<string, object> m_Resources;
        private (string Name, object Data)[] m_Inheritances;

        /// <summary>
        /// Kernel instance.
        /// </summary>
        /// <param name="Engine"></param>
        public Kernel(Engine Engine)
        {
            List<Kernel> Kernels = TlsVariables
                .Get<List<Kernel>>("http-kernel");

            if (Kernels is null)
            {
                /* *
                 * For supporting multi-port.
                 * */
                TlsVariables.Set("http-kernel",
                    Kernels = new List<Kernel>());
            }

            m_Listener = new List<IListener>();
            m_Middleware = new List<IMiddleware>();
            m_Resources = new ConcurrentDictionary<string, object>();

            m_Event = new ManualResetEvent(false);
            m_Thread = new Thread(OnThreadMain);

            Kernels.Add(this);
        }
        
        /// <summary>
        /// Kernel dtor.
        /// </summary>
        ~Kernel() => Stop();

        /// <summary>
        /// Engine instance.
        /// </summary>
        public Engine Engine { get; private set; }

        /// <summary>
        /// Make this kernel to use given listener.
        /// </summary>
        /// <param name="Listener"></param>
        /// <returns></returns>
        public Kernel Use(IListener Listener)
        {
            if (m_Thread.IsAlive)
            {
                throw new InvalidOperationException(
                    "Can't modify Kernel if engine has started.");
            }

            lock (m_Listener)
                m_Listener.Add(Listener);

            return this;
        }

        /// <summary>
        /// Make this kernel to co-operate with middleware.
        /// (global-scope middleware)
        /// </summary>
        /// <param name="Middleware"></param>
        /// <returns></returns>
        public Kernel With(IMiddleware Middleware)
        {
            if (m_Thread.IsAlive)
            {
                throw new InvalidOperationException(
                    "Can't modify Kernel if engine has started.");
            }

            lock (m_Middleware)
                m_Middleware.Add(Middleware);

            return this;
        }

        /// <summary>
        /// Map resource to target path.
        /// </summary>
        /// <param name="BasePath"></param>
        /// <param name="Router"></param>
        /// <returns></returns>
        public Kernel Map(string BasePath, object Resource)
        {
            m_Resources[BasePath] = Resource;
            return this;
        }

        /// <summary>
        /// Set this kernel to start to get context.
        /// </summary>
        public void Start()
        {
            m_Inheritances = TlsVariables.Export();
            m_Thread.Start();
        }

        /// <summary>
        /// Set this kernel to stop getting context.
        /// Terminating stages are programmed by Dtors.
        /// </summary>
        public void Stop()
        {
            try
            {
                if (m_Thread.IsAlive)
                {
                    m_Event.Set();
                    m_Thread.Join();
                }
            }

            catch { }
        }

        /// <summary>
        /// Thread main.
        /// </summary>
        private void OnThreadMain()
        {
            TlsVariables
                .Import(m_Inheritances);
            m_Inheritances = null;

            while (!m_Event.WaitOne(0))
            {
                IRequest Request = null;
                Context Context;

                foreach(var Each in m_Listener)
                {
                    if (Each.HasPending())
                    {
                        try { Request = Each.Accept(0); }
                        catch { }

                        break;
                    }
                }

                if (Request is null)
                {
                    Thread.Sleep(10);
                    continue;
                }

                try { Context = new Context(Request, new Response()); }
                catch { continue; }

                /*
                 * Handle a context and then,
                 * commit response to client.
                 */
                Future<Context> Future = Handle(Context);

                Future.And(new Future(() => Commit(Context)))
                    .Schedule();
            }
        }

        /// <summary>
        /// Commit Response to real response.
        /// </summary>
        /// <param name="Context"></param>
        private void Commit(Context Context)
        {
            var Resp = (Context.Request.Connection as Connection)
                               .HLC.Response;

            if (Context.Request.Method == EMethod.OPTIONS)
                Context.Response.StatusCode = EStatusCode.NoContent;

            try { Resp.StatusCode = (int)Context.Response.StatusCode; }
            catch { }

            Context.Response.Headers.Export(Resp);

            /* Set-up CORS headers. */
            Resp.Headers.Add("Access-Control-Allow-Origin", "*");
            Resp.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, PATCH, DELETE, OPTIONS");
            Resp.Headers.Add("Access-Control-Allow-Headers", "Authorization");
            Resp.Headers.Add("Access-Control-Max-Age", "60");

            if (!(Context.Response.Output.Content is null))
            {
                var Output = Context.Response.Output;

                if (Output.MimeType == "application/octet-stream")
                    Resp.ContentType = "application/octet-stream";

                else Resp.ContentType = string.Join("; ", Output.MimeType ?? "text/plain",
                    string.Join('=', "charset", (Output.Encoding ?? Encoding.UTF8).WebName));

                try
                {
                    if (Output.Length == 0 && Output.Offset == 0)
                        Output.Content.CopyTo(Resp.OutputStream);

                    else
                    {
                        byte[] Buffer = new byte[8192];
                        Output.Content.Seek(Output.Offset, SeekOrigin.Begin);

                        while (Output.Length > 0)
                        {
                            int Reads = Output.Content.Read(Buffer);

                            Output.Length -= Reads;
                            Output.Offset += Reads;

                            Resp.OutputStream.Write(Buffer, 0, Reads);
                        }
                    }
                }
                catch { }

                try { Context.Response.Output.Close(Output.Content); }
                catch { }
            }

            Context.Request.Connection.Commit();

            foreach (var Each in Context.Response.Headers)
                Header.DeInstantiate(Each);

            foreach (var Each in Context.Request.Headers)
                Header.DeInstantiate(Each);

            Context.Request.Headers.Clear();
            Context.Response.Headers.Clear();
        }

        /// <summary>
        /// Context handler.
        /// </summary>
        /// <param name="Context"></param>
        private Context OnContext(Context Context)
        {
            object TargetObject = null;
            string BasePathName = null;

            foreach(string BasePath in m_Resources.Keys)
            {
                if (Context.Request.Path.StartsWith(BasePath))
                {
                    TargetObject = m_Resources[BasePath];
                    BasePathName = BasePath;
                    break;
                }
            }


            if (TargetObject is null)
                return Context;

            else if (TargetObject is IHandler)
            {
                Context.Response.StatusCode = EStatusCode.Okay;

                try { return (TargetObject as IHandler).Handle(Context); }
                catch
                {
                    Context.Response.StatusCode = EStatusCode.InternalServerError;
                    Context.Response.Output = new ResponseContent { };
                }
            }

            /* *
             * Input path is mapped for really existed file.
             * */
            else if (TargetObject is FileInfo)
            {
                try { SetContentAsFile(Context, TargetObject as FileInfo); }
                catch
                {
                    Context.Response.StatusCode = EStatusCode.InternalServerError;
                    Context.Response.Output = new ResponseContent { };
                }
            }

            /* *
             * Input path is mapped for really existed directory.
             * */
            else if (TargetObject is DirectoryInfo)
            {
                DirectoryInfo Directory = TargetObject as DirectoryInfo;
                string PathName = Path.Combine(Directory.FullName,
                    Context.Request.Path.Substring(BasePathName.Length).Trim('/').Trim());

                if (File.Exists(PathName))
                {
                    try { SetContentAsFile(Context, new FileInfo(PathName)); }
                    catch
                    {
                        Context.Response.StatusCode = EStatusCode.InternalServerError;
                        Context.Response.Output = new ResponseContent { };
                    }
                }
            }

            return Context;
        }

        /// <summary>
        /// Set content from file.
        /// </summary>
        /// <param name="Context"></param>
        /// <param name="File"></param>
        private static void SetContentAsFile(Context Context, FileInfo File)
        {
            switch (File.Extension.ToLower())
            {
                case ".jpg":
                case ".jpeg":
                case ".png":
                case ".gif":
                    SetContentAsFile(Context, File, "image");
                    break;

                case ".css":
                case ".js":
                    SetContentAsFile(Context, File, "text");
                    break;

                case ".json":
                case ".xml":
                    SetContentAsFile(Context, File, "application");
                    break;

                case ".txt":
                case ".log":
                    SetContentAsFile(Context, File, "text", "plain");
                    break;

                case ".html":
                case ".htm":
                    SetContentAsFile(Context, File, "text", "html");
                    break;

                default:
                    SetContentAsFile(Context, File, "application", "octet-stream");
                    break;
            }
        }

        /// <summary>
        /// Set content from file.
        /// </summary>
        /// <param name="Context"></param>
        /// <param name="File"></param>
        private static void SetContentAsFile(Context Context, FileInfo File, string Prefix, string Postfix = null)
        {
            Context.Response.StatusCode = EStatusCode.Okay;
            Context.Response.Output = new ResponseContent
            {
                Content = File.OpenRead(),
                MimeType = Prefix + "/" + (Postfix ?? File.Extension.ToLower()),
                Encoding = Encoding.UTF8,
                Length = 0,
                Offset = 0,
                Close = X => X.Dispose()
            };
        }

        /// <summary>
        /// Handle context with middle-wares.
        /// </summary>
        /// <param name="Context"></param>
        /// <returns></returns>
        private Future<Context> Handle(Context Context)
        {
            Future<Context> First = null;
            
            if (Context.Request.Method == EMethod.OPTIONS)
                return Future.MakeCompleted(Context);

            Context.Response.StatusCode = EStatusCode.NotFound;

            foreach (var Each in m_Middleware)
            {
                Future<Context> Current = Each.Handle(Context);

                if (First is null)
                    First = Current;

                else First = First.Then(Current);
            }

            if (First is null)
                return new Future<Context>(() => OnContext(Context));

            return First.Then(new Future<Context>(() => OnContext(Context)));
        }
    }
}
