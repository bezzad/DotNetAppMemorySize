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

            await Console.Out.WriteLineAsync("Do you want to repeat the tests? (y/n)").ConfigureAwait(false);
            if (Console.ReadKey().KeyChar is 'y' or 'Y')
                continue;

            break;
        }

        Console.WriteLine("============ END ===========");
        Console.ReadLine();
    }
}