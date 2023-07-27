using Microsoft.Diagnostics.Runtime;
using System.Diagnostics;

namespace TestZombieThread
{
    public class MemoryCalc
    {
        Dictionary<ulong, (int Count, ulong Size, string Name)> stats = new Dictionary<ulong, (int Count, ulong Size, string Name)>();

        public int GetRuntimeAppThreads()
        {
            using Process currentProcess = Process.GetCurrentProcess();
            using DataTarget target = DataTarget.CreateSnapshotAndAttach(currentProcess.Id);
            using ClrRuntime runtime = target.ClrVersions.First().CreateRuntime();
            var heap = runtime.Heap;
            return runtime.Threads.Count();
        }

        public (int Count, double Size) GetObjectsSizes()
        {
            using Process currentProcess = Process.GetCurrentProcess();
            using DataTarget target = DataTarget.CreateSnapshotAndAttach(currentProcess.Id); // DataTarget.AttachToProcess(currentProcess.Id, false)
            using ClrRuntime runtime = target.ClrVersions.First().CreateRuntime();
            var heap = runtime.Heap;

            if (!heap.CanWalkHeap)
            {
                Console.WriteLine("Cannot walk the heap!");
                return (0, 0);
            }
            else
            {
                foreach (ClrObject obj in heap.EnumerateObjects())
                {
                    if (obj.Type == null) continue;

                    //Console.WriteLine($"{obj.Address:x16} {obj.Type.MethodTable:x16} {obj.Size,8:D} {obj.Type.Name}");

                    if (!stats.TryGetValue(obj.Type.MethodTable, out (int Count, ulong Size, string Name) item))
                        item = (0, 0, obj.Type.Name);

                    ulong objSize = obj.Size; //  Helper.ObjSize(obj); 
                    stats[obj.Type.MethodTable] = (item.Count + 1, item.Size + objSize, item.Name);
                }

                var sorted = from i in stats
                             orderby i.Value.Size ascending
                             select new
                             {
                                 i.Key,
                                 i.Value.Name,
                                 i.Value.Size,
                                 i.Value.Count
                             };

                //Console.WriteLine("{0,16} {1,12} {2,12}\t{3}", "MethodTable", "Count", "Size", "Type");
                //foreach (var item in sorted)
                //    Console.WriteLine($"{item.Key:x16} {item.Count,12:D} {item.Size,12:D}\t{item.Name}");

                var result = (sorted.Sum(x => x.Count), sorted.Sum(x => (double)x.Size));
                Console.WriteLine($"Total {result.Item1:0} objects  | {result.Item2.ConvertToHumanReadable()}");
                return result;
            }
        }


        public long GetTotalAllocatedBytes()
        {
            var result = GC.GetTotalAllocatedBytes();
            Console.WriteLine($"GC Total allocated memory: {result.ConvertToHumanReadable()}");
            return result;
        }

        public long GetTotalMemory()
        {
            var result = GC.GetTotalMemory(false);
            Console.WriteLine($"GC Total memory: {result.ConvertToHumanReadable()}");
            return result;
        }
    }
}
