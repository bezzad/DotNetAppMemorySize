using System.Collections.Concurrent;
using System.Diagnostics;

namespace TestZombieThread
{
    public static class Test
    {
        static int threadsCount = 1_000;
        static int tasksCount = 100_000;
        static ThreadProducer threadProducer = new ThreadProducer();
        static ConcurrentDictionary<int, long> array = new ConcurrentDictionary<int, long>();

        public static async Task CreatingZombieThreads()
        {
            await Console.Out.WriteAsync($"\nCreating {threadsCount:N0} zombie threads");
            for (int i = 0; i < threadsCount; i++)
            {
                threadProducer.CreateThread();
                await Counting(i).ConfigureAwait(false);
            }

            await Run().ConfigureAwait(false);
        }

        public static async Task ReleaseZombieThreads()
        {
            await Console.Out.WriteAsync("\n\nRelease zombie threads...  \n");
            ThreadProducer.CanContinue = true;
            await Task.Delay(1000);
            GC.Collect();
            await Run().ConfigureAwait(false);
        }

        public static async Task AddingTestData()
        {
            await Console.Out.WriteAsync("\n\nAdding test data");
            for (int i = 0; i < 20_000_000; i++)
            {
                array[i] = i;
                await Counting(i, 100_000).ConfigureAwait(false);
            }
            await Console.Out.WriteLineAsync($"\n{array.Count():N0} items added.\n").ConfigureAwait(false);
            await Run().ConfigureAwait(false);
        }

        public static async Task OpenCloseThreads()
        {
            await Console.Out.WriteAsync($"\nOpen and close new {threadsCount:N0} threads");
            for (int i = 0; i < threadsCount; i++)
            {
                threadProducer.CreateThread();
                await Counting(i).ConfigureAwait(false);
            }

            GC.Collect();
            await Run().ConfigureAwait(false);
        }

        public static async Task OpenCloseTasks()
        {
            await Console.Out.WriteAsync($"\nCreating {tasksCount:N0} tasks").ConfigureAwait(false);
            var tasks = new List<Task>();
            for (int i = 0; i < tasksCount; i++)
            {
                tasks.Add(threadProducer.CreateTask());
                await Counting(i, 1000).ConfigureAwait(false);
            }

            await Run().ConfigureAwait(false);

            // await Parallel.ForEachAsync(tasks.ToArray()); // open thread (speed up) 1000 count | 10,000            
            await Task.WhenAll(tasks).ConfigureAwait(false); // thread pool (scalability up) sharing resources - ThreadPool.QueueUserWorkItem

            GC.Collect();
            await Run().ConfigureAwait(false);
        }

        private static async Task Counting(int i, int per = 100)
        {
            if (i % per == 0)
                await Console.Out.WriteAsync(".").ConfigureAwait(false);
        }

        public static async Task Run()
        {
            await Task.Delay(10).ConfigureAwait(false);
            using Process currentProcess = Process.GetCurrentProcess();
            var memCalc = new MemoryCalc();
            ThreadPool.GetAvailableThreads(out var workerThreads, out var completionPortThreads);

            await Console.Out.WriteLineAsync("\n------------ Info ------------").ConfigureAwait(false);
            await Console.Out.WriteLineAsync($"Process {currentProcess.ProcessName} #{currentProcess.Id} --> ManagedThread #{Thread.CurrentThread.ManagedThreadId} - Threads {currentProcess.Threads.Count} | Runtime Threads {memCalc.GetRuntimeAppThreads()}").ConfigureAwait(false);
            await Console.Out.WriteLineAsync($"ThreadPool threads of current {ThreadPool.ThreadCount} | workers {workerThreads} | " +
                $"completed {ThreadPool.CompletedWorkItemCount} | pending {ThreadPool.PendingWorkItemCount}").ConfigureAwait(false);

            memCalc.GetTotalAllocatedBytes();
            memCalc.GetTotalMemory();
            memCalc.GetObjectsSizes();
            await Console.Out.WriteLineAsync("------------------------------").ConfigureAwait(false);
        }
    }
}
