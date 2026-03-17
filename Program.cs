using System;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello APBD Git!");
        Console.WriteLine("Task 2: First small change");

        GreetUser("Taha");

        int userAge = 21;
        Console.WriteLine($"User age: {userAge}");

        string userName = "";
        if (string.IsNullOrEmpty(userName))
        {
            Console.WriteLine("Error: userName is empty!");
        }
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
}