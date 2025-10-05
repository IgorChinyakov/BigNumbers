using BigNumbersLib;

internal class Program
{
    private static void Main()
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        var value = new BigNumber("2");
        value = (value ^ 136279841) - new BigNumber("1");
        watch.Stop();

        Console.WriteLine(watch.Elapsed.TotalMinutes);
    }
}