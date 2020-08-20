using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;

namespace Aegis.Workers.Tasks
{
    /// <summary>
    /// Future class for predicatable worker.
    /// </summary>
    public class Future
    {
        internal IWorker m_Worker;
        private Action m_Functor;

        internal Queue<Future> m_Futures;
        internal int m_Scheduled = 0;
        internal int m_Completion = 0;

        internal ManualResetEventSlim m_Waiter;
        internal Exception m_Exception;

        /// <summary>
        /// Already Completed Future.
        /// </summary>
        public static Future Completed { get; } = new Future();

        /// <summary>
        /// Already Faulted Future.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static Future<TResult> MakeCompleted<TResult>(TResult e) => new Future<TResult>(e);

        /// <summary>
        /// Already Faulted Future.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static Future MakeFaulted(Exception e) => new Future() { m_Exception = e };

        /// <summary>
        /// Already Faulted Future.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static Future<TResult> MakeFaulted<TResult>(Exception e) => new Future<TResult>() { m_Exception = e };

        /// <summary>
        /// Initialize a future with functor.
        /// </summary>
        /// <param name="Functor"></param>
        public Future(Action Functor)
        {
            if ((m_Worker = TlsVariables.Get<IWorker>("worker")) is null)
                throw new InvalidOperationException("Future class requires valid worker instance!");

            m_Futures = new Queue<Future>();
            m_Functor = Functor;

            m_Waiter = new ManualResetEventSlim(false);
        }

        /// <summary>
        /// Initialize an already completed future.
        /// </summary>
        /// <param name="RequiresWorker"></param>
        internal Future(bool Scheduled = true, bool Completed = true)
        {
            if (!Scheduled && (m_Worker = TlsVariables.Get<IWorker>("worker")) is null)
                throw new InvalidOperationException("Future class requires valid worker instance!");

            if (!Scheduled || !Completed)
                m_Futures = new Queue<Future>();

            m_Scheduled = Scheduled ? 1 : 0;
            m_Completion = Completed ? 1 : 0;

            m_Waiter = new ManualResetEventSlim(Completed);
        }

        /// <summary>
        /// Schedule this future task to be ran.
        /// </summary>
        /// <returns></returns>
        public Future Schedule()
        {
            if (Interlocked.Increment(ref m_Scheduled) == 1)
                m_Worker.Enqueue(OnFutureRun, this);

            return this;
        }

        /// <summary>
        /// If this future completed then, schedule given future.
        /// (Linear chaining)
        /// </summary>
        /// <param name="Future"></param>
        /// <returns></returns>
        public Future Then(Future Future)
        {
            if (Future is null)
                throw new ArgumentNullException(nameof(Future));

            bool Immediate = false;
            lock(this)
            {
                if (m_Completion <= 0)
                    m_Futures.Enqueue(Future);

                else Immediate = true;
            }

            if (Immediate)
                Future.Schedule();

            return Future;
        }

        /// <summary>
        /// Determines this task completed or not.
        /// </summary>
        public bool IsCompleted => m_Completion != 0;

        /// <summary>
        /// Determines this task completed but, exception occurred or not.
        /// </summary>
        public bool IsFaulted => IsCompleted && Exception != null;

        /// <summary>
        /// Exception if occurred during execution.
        /// </summary>
        public Exception Exception => m_Exception;

        /// <summary>
        /// Wait completion of this future.
        /// </summary>
        /// <returns></returns>
        public bool Wait(bool Schedule = false)
        {
            if (m_Scheduled <= 0)
            {
                return Schedule ? 
                    this.Schedule() == this : 
                    false;
            }

            m_Waiter.Wait();
            return true;
        }

        /// <summary>
        /// If this future completed then, schedule given future.
        /// (Linear chaining)
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="Future"></param>
        /// <returns></returns>
        public Future<TResult> Then<TResult>(Future<TResult> Future) 
            => Then((Future)Future) as Future<TResult>;

        /// <summary>
        /// If this future completed then, schedule given future.
        /// (Pararel chaining)
        /// </summary>
        /// <param name="Future"></param>
        /// <returns></returns>
        public Future And(Future Future)
        {
            Then(Future);
            return this;
        }

        /// <summary>
        /// Executes a future on worker thread.
        /// </summary>
        /// <param name="state"></param>
        private static void OnFutureRun(object state)
        {
            var Future = (state as Future);

            try { Future.OnExecution(); }
            catch (Exception e) { 
                Future.m_Exception = e; 
            }

            Future.m_Waiter.Set();
            lock (Future)
            {
                ++Future.m_Completion;

                while (Future.m_Futures.Count > 0)
                    Future.m_Futures.Dequeue().Schedule();
            }
        }
        
        /// <summary>
        /// Callback for executing the functor.
        /// </summary>
        protected virtual void OnExecution() => m_Functor();
    }

    public class Future<TResult> : Future
    {
        private Func<TResult> m_Functor;
        internal TResult m_Result;

        /// <summary>
        /// Initialize a future with functor.
        /// </summary>
        /// <param name="Functor"></param>
        public Future(Func<TResult> Functor)
            : base(() => { })
        {
            m_Functor = Functor;
        }

        /// <summary>
        /// Initialize an already completed future.
        /// </summary>
        /// <param name="Dummy"></param>
        public Future(TResult Result)
            : base()
        {
            m_Result = Result;
        }

        /// <summary>
        /// Initialize an already completed future.
        /// </summary>
        /// <param name="RequiresWorker"></param>
        internal Future(bool Scheduled = true, bool Completed = true) 
            : base(Scheduled, Completed)
        {
        }

        /// <summary>
        /// If this future completed then, schedule given future.
        /// (Pararel chaining)
        /// </summary>
        /// <param name="Future"></param>
        /// <returns></returns>
        public new Future<TResult> And(Future Future)
        {
            Then(Future);
            return this;
        }

        /// <summary>
        /// Obtains the execution result.
        /// This value is invalid if IsFaulted == true.
        /// </summary>
        public TResult Result {
            get {
                if (!Wait())
                    throw new InvalidOperationException("Future object must be scheduled!");

                return m_Result;
            }
        }

        /// <summary>
        /// Wait completion of this future.
        /// </summary>
        /// <param name="Result"></param>
        /// <returns></returns>
        public bool Wait(out TResult Result)
        {
            if (Wait())
            {
                Result = m_Result;
                return true;
            }

            Result = default;
            return false;
        }

        /// <summary>
        /// Callback for executing the functor.
        /// </summary>
        protected override void OnExecution() => m_Result = m_Functor();
    }
}
