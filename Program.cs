Console.WriteLine("Hello APBD Git!");
Console.WriteLine("Task 2: First small change");

static void GreetUser(string name)
{
    Console.WriteLine($"Hello, {name}!");
}

int userAge = 21;
Console.WriteLine($"User age: {userAge}");

string userName = "";
if(string.IsNullOrEmpty(userName))
{
    Console.WriteLine("Error: userName is empty!");
}