namespace TestZombieThread
{
    public class ThreadProducer
    {
        public static volatile bool CanContinue = false;
        public static volatile Dictionary<long, Thread> Threads = new Dictionary<long, Thread>();

        public void CreateThread()
        {
            var thread = new Thread(Job);
            Threads[thread.ManagedThreadId] = thread;
            thread.Start();
        }

        public void CreateTask()
        {
            var task = new Task(async ()=>
            {
                await Task.Delay(100);
            });
            task.Start();
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

            Threads.Remove(Thread.CurrentThread.ManagedThreadId);
        }
    }
}
