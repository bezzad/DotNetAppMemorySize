// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using TestZombieThread;

using Process currentProcess = Process.GetCurrentProcess();
var memCalc = new MemoryCalc();

Console.WriteLine($"Process {currentProcess.ProcessName} - #{currentProcess.Id} - " +
    $"Threads {currentProcess.Threads.Count} | Runtime Threads {memCalc.GetRuntimeAppThreads()}");

memCalc.GetTotalAllocatedBytes();
memCalc.GetTotalMemory();
memCalc.GetObjectsSizes();

Console.Write("\n\nAdding test data...  ");
var array = new Dictionary<int, long>(1000);
for (int i = 0; i < 200_000_000; i++)
{
    array[i] = (long)i;
}
Console.WriteLine($"\t {array.Count()} added.");

memCalc.GetTotalAllocatedBytes();
memCalc.GetTotalMemory();
memCalc.GetObjectsSizes();

Console.ReadLine();