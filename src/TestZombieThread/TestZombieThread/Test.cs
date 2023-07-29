using System.Diagnostics;

namespace TestZombieThread
{
    public static class Test
    {
        static int threadsCount = 1_000;
        static int tasksCount = 100_000;
        static ThreadProducer threadProducer = new ThreadProducer();
        static Dictionary<int, long> array = new Dictionary<int, long>();

        public static void CreatingZombieThreads()
        {
            Console.Write($"\nCreating {threadsCount:N0} zombie threads");
            for (int i = 0; i < threadsCount; i++)
            {
                threadProducer.CreateThread();
                Counting(i);
            }

            Run();
        }

        public static void ReleaseZombieThreads()
        {
            Console.Write("\n\nRelease zombie threads...  \n");
            ThreadProducer.CanContinue = true;

            Run();
        }

        public static void AddingTestData()
        {
            Console.Write("\n\nAdding test data");
            for (int i = 0; i < 20_000_000; i++)
            {
                array[i] = i;
                Counting(i, 100_000);
            }
            Console.WriteLine($"\n{array.Count():N0} items added.\n");
            Run();
        }

        public static void OpenCloseThreads()
        {
            Console.Write($"\nOpen and close new {threadsCount:N0} threads");
            for (int i = 0; i < threadsCount; i++)
            {
                threadProducer.CreateThread();
                Counting(i);
            }

            GC.Collect();
            Run();
        }

        public static async Task OpenCloseTasks()
        {
            Console.Write($"\nCreating {tasksCount:N0} tasks");
            var tasks = new List<Task>();
            for (int i = 0; i < tasksCount; i++)
            {
                tasks.Add(threadProducer.CreateTask());
                Counting(i, 1000);
            }

            Run();
            await Task.WhenAll(tasks).ConfigureAwait(false);
            GC.Collect();
            Run();
        }

        private static void Counting(int i, int per = 100)
        {
            if (i % per == 0)
                Console.Write(".");
        }

        public static void Run()
        {
            Thread.Sleep(1000);
            using Process currentProcess = Process.GetCurrentProcess();
            var memCalc = new MemoryCalc();
            ThreadPool.GetAvailableThreads(out var workerThreads, out var completionPortThreads);

            Console.WriteLine("\n------------ Info ------------");
            Console.WriteLine($"Process {currentProcess.ProcessName} #{currentProcess.Id} --> ManagedThread #{Thread.CurrentThread.ManagedThreadId} - Threads {currentProcess.Threads.Count} | Runtime Threads {memCalc.GetRuntimeAppThreads()}");
            Console.WriteLine($"ThreadPool threads of current {ThreadPool.ThreadCount} | workers {workerThreads} | " +
                $"completed {ThreadPool.CompletedWorkItemCount} | pending {ThreadPool.PendingWorkItemCount}");

            memCalc.GetTotalAllocatedBytes();
            memCalc.GetTotalMemory();
            memCalc.GetObjectsSizes();
            Console.WriteLine("------------------------------");
        }
    }
}
