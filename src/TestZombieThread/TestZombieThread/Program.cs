// See https://aka.ms/new-console-template for more information
using TestZombieThread;

var threads = 10_000;
var threadProducer = new ThreadProducer();
Test.Run();

Console.WriteLine($"\nCreating {threads} zombie threads...\n");
for (int i = 0; i < threads; i++)
{
    threadProducer.CreateThread();
}

Test.Run();

Console.Write("\n\nRelease zombie threads...  \n");
ThreadProducer.CanContinue = true;

Test.Run();

Console.Write("\n\nAdding test data...  ");
var array = new Dictionary<int, long>(1000);
for (int i = 0; i < 20_000_000; i++)
{
    array[i] = i;
}
Console.WriteLine($"\t {array.Count()} added.\n");

Test.Run();


Console.WriteLine($"\nOpen and close new {threads} threads...\n");
for (int i = 0; i < threads; i++)
{
    threadProducer.CreateThread();
}

GC.Collect();
Test.Run();

Console.WriteLine($"\nCreating {threads} tasks...\n");
for (int i = 0; i < threads; i++)
{
    threadProducer.CreateTask();
}

GC.Collect();
Test.Run();

Console.WriteLine("============ END ===========");
Console.ReadLine();