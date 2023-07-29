using System.Collections.Concurrent;

namespace TestZombieThread
{
    public class ThreadProducer
    {
        public static volatile bool CanContinue = false;
        public static volatile ConcurrentDictionary<long, Thread> Threads = new ConcurrentDictionary<long, Thread>();

        public void CreateThread()
        {
            var thread = new Thread(Job);
            Threads.TryAdd(thread.ManagedThreadId, thread);
            thread.Start();
        }

        public Task CreateTask()
        {
            return Task.Run(async ()=>
            {
                await Task.Delay(100);
                await Task.Yield();
            });
        }

        private void Job()
        {
            var obj = new
            {
                X = "testX",
                Y = "testY",
                Z = "testZ"
            };

            Monitor.Enter(this);

            while (!CanContinue)
            {
                Thread.Sleep(100);
            }

            Monitor.Exit(this);

            Threads.TryRemove(Thread.CurrentThread.ManagedThreadId, out var _);
        }
    }
}
