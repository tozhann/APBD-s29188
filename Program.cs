using System;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("This change is from main branch");

        GreetUser("Taha");

        int userAge = 21;
        Console.WriteLine($"User age: {userAge}");

        string userName = "";
        if (string.IsNullOrEmpty(userName))
        {
            Console.WriteLine("Error: userName is empty!");
        }

        int[] values = { 5, 10, 3, 8 };
        Console.WriteLine($"Average: {CalculateAverage(values)}");
        Console.WriteLine($"Maximum: {CalculateMax(values)}");
        Console.WriteLine($"Minimum: {CalculateMin(values)}");
    }

    static void GreetUser(string name)
    {
        Console.WriteLine($"Hello, {name}!");
    }

    static double CalculateAverage(int[] values)
    {
        double sum = 0;
        foreach (var v in values)
            sum += v;
        return sum / values.Length;
    }

    static int CalculateMax(int[] values)
    {
        int max = values[0];
        foreach (var v in values)
            if (v > max) max = v;
        return max;
    }

    static int CalculateMin(int[] values)
    {
        int min = values[0];
        foreach (var v in values)
            if (v < min) min = v;
        return min;
    }
}