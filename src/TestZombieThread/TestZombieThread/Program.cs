namespace TestZombieThread;

public class Progrm
{
    static int threads = 1_000;
    static ThreadProducer threadProducer = new ThreadProducer();
    static Dictionary<int, long> array = new Dictionary<int, long>();

    public static async Task Main()
    {
        Test.Run();
        CreatingZombieThreads();
        ReleaseZombieThreads();
        AddingTestData();
        OpenCloseThreads();
        await OpenCloseTasks().ConfigureAwait(false);

        Console.WriteLine("============ END ===========");
        Console.ReadLine();
    }

    public static void CreatingZombieThreads()
    {
        Console.Write($"\nCreating {threads} zombie threads");
        for (int i = 0; i < threads; i++)
        {
            threadProducer.CreateThread();
            Counting(i);
        }

        Test.Run();
    }

    public static void ReleaseZombieThreads()
    {
        Console.Write("\n\nRelease zombie threads...  \n");
        ThreadProducer.CanContinue = true;

        Test.Run();
    }

    public static void AddingTestData()
    {
        Console.Write("\n\nAdding test data");
        for (int i = 0; i < 20_000_000; i++)
        {
            array[i] = i;
            Counting(i, 100_000);
        }
        Console.WriteLine($"\n{array.Count()} items added.\n");

        Test.Run();
    }

    public static void OpenCloseThreads()
    {
        Console.Write($"\nOpen and close new {threads} threads");
        for (int i = 0; i < threads; i++)
        {
            threadProducer.CreateThread();
            Counting(i);
        }

        GC.Collect();
        Test.Run();
    }

    public static async Task OpenCloseTasks()
    {
        Console.Write($"\nCreating {threads} tasks");
        var tasks = new List<Task>();
        for (int i = 0; i < threads; i++)
        {
            tasks.Add(threadProducer.CreateTask());
            Counting(i);
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);
        GC.Collect();
        Test.Run();
    }

    private static void Counting(int i, int per = 100)
    {
        if (i % per == 0)
            Console.Write(".");
    }
}
