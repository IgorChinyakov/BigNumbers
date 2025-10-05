using BigNumbersLib;

internal class Program
{
    private static void Main()
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        BigNumber firstBigNumber = new BigNumber(File.ReadAllText("FirstNumber.txt"));
        BigNumber secondBigNumber = new BigNumber(File.ReadAllText("SecondNumber.txt"));

        //BigNumber firstBigNumber = new("1000000");
        //BigNumber secondBigNumber = new("125575");

        int degree = 450;

        var sum = secondBigNumber + firstBigNumber;
        var subtraction = secondBigNumber - firstBigNumber;
        var multiplication = firstBigNumber * secondBigNumber;
        var division = firstBigNumber / secondBigNumber;
        var exponention = secondBigNumber ^ degree;
        var GCD = BigNumberMath.GCD(firstBigNumber, secondBigNumber);
        var LCM = BigNumberMath.LCM(firstBigNumber, secondBigNumber);

        watch.Stop();

        File.WriteAllText("Multiplication.txt", multiplication.ToString());
        File.WriteAllText("Division.txt", division.ToString());
        File.WriteAllText("Exponention.txt", exponention.ToString());
        File.WriteAllText("Sum.txt", sum.ToString());
        File.WriteAllText("Subtraction.txt", subtraction.ToString());
        File.WriteAllText("GCD.txt", GCD.ToString());
        File.WriteAllText("LCM.txt", LCM.ToString());

        Console.WriteLine("Less: " + (firstBigNumber < secondBigNumber).ToString());
        Console.WriteLine("More: " + (firstBigNumber > secondBigNumber).ToString());
        Console.WriteLine("Equal: " + (firstBigNumber == secondBigNumber).ToString());
        Console.WriteLine("NotEqual: " + (firstBigNumber != secondBigNumber).ToString());
        Console.WriteLine("LessOrEqual: " + (firstBigNumber <= secondBigNumber).ToString());
        Console.WriteLine("MoreOrEqual: " + (firstBigNumber >= secondBigNumber).ToString());

        Console.WriteLine(watch);
        Console.ReadLine();
    }
}