namespace TestZombieThread;

public class Progrm
{
    public static async Task Main()
    {
        while (true)
        {
            GC.Collect();
            Console.Clear();
            await Test.Run().ConfigureAwait(false);

            await Test.CreatingZombieThreads().ConfigureAwait(false);
            await Test.ReleaseZombieThreads().ConfigureAwait(false);
            await Test.AddingTestData().ConfigureAwait(false);
            await Test.OpenCloseThreads().ConfigureAwait(false);
            await Test.OpenCloseTasks().ConfigureAwait(false);
            await Test.Run().ConfigureAwait(false); // 1
            await Console.Out.WriteLineAsync("Do you want to repeat the tests? (y/n)").ConfigureAwait(false); // 2
            if (Console.ReadKey().KeyChar is 'y' or 'Y') // true: 1   |  false: 2
                continue;
            break;
        }

        Console.WriteLine("============ END ===========");
        Console.ReadLine();
    }
}