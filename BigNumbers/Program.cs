using BigNumbers;

internal class Program
{
    private static void Main()
    {
        BigNumber firstBigNumber = new BigNumber(File.ReadAllText("FirstNumber.txt"));
        //BigNumber secondBigNumber = new BigNumber(File.ReadAllText("Multiplication.txt"));

        //BigNumber firstBigNumber = new("818242");
        BigNumber secondBigNumber = new("10");

        var result = firstBigNumber < secondBigNumber;

        var sum = secondBigNumber + firstBigNumber;
        var subtraction = secondBigNumber - firstBigNumber;
        var multiplication = firstBigNumber * secondBigNumber;
        var division = secondBigNumber / firstBigNumber;
        var degree = secondBigNumber ^ 340;

        File.WriteAllTextAsync("Multiplication.txt", multiplication.ToString());
        File.WriteAllTextAsync("Division.txt", division.ToString());
        File.WriteAllTextAsync("Degree.txt", degree.ToString());
        File.WriteAllTextAsync("Sum.txt", sum.ToString());
        File.WriteAllTextAsync("Subtraction.txt", subtraction.ToString());
    }
}