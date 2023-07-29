namespace TestZombieThread;

public class Progrm
{
    public static async Task Main()
    {
        Test.Run();
        Test.CreatingZombieThreads();
        Test.ReleaseZombieThreads();
        Test.AddingTestData();
        Test.OpenCloseThreads();
        await Test.OpenCloseTasks().ConfigureAwait(false);

        Console.WriteLine("============ END ===========");
        Console.ReadLine();
    }
}