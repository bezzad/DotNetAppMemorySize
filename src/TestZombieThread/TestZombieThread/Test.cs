using System.Diagnostics;

namespace TestZombieThread
{
    public static class Test
    {
        public static void Run()
        {
            Thread.Sleep(1000);
            using Process currentProcess = Process.GetCurrentProcess();
            var memCalc = new MemoryCalc();

            Console.WriteLine("------------ Info ------------");
            Console.WriteLine($"Process {currentProcess.ProcessName} - #{currentProcess.Id} - " +
    $"Threads {currentProcess.Threads.Count} | Runtime Threads {memCalc.GetRuntimeAppThreads()}");

            memCalc.GetTotalAllocatedBytes();
            memCalc.GetTotalMemory();
            memCalc.GetObjectsSizes();
            Console.WriteLine("------------------------------");
        }
    }
}
