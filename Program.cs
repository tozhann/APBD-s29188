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
    
    foreach (var v in values)
    if (v < min) min = v;
    
    static double CalculateAverage(int[] values)
    {
        double sum = 0;
        foreach (var v in values)
            sum += v;
        return sum / values.Length;
    }
    
    static int CalculateMin(int[] values)
    {
        int min = values[0];
            
<<<<<<< HEAD
    Console.WriteLine("Minor update on main branch");
=======
    
    static int CalculateMax(int[] values)
    {
        int max = values[0];
        foreach (var v in values)
            if (v > max) max = v;
        return max;
    }
>>>>>>> feature-max
}