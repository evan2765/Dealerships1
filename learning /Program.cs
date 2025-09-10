/*using System;
using System.Collections.Generic;

public class Program
{
    static void Main(string[] args)
    {
        var shouldRun = true;

        while (shouldRun)
        {
            Console.WriteLine("Welcome to the calculator!");

            Console.WriteLine("Choose a function:");
            Console.WriteLine("1 - Add");
            Console.WriteLine("2 - Subtract");
            Console.WriteLine("3 - Multiply");      
            Console.WriteLine("4 - Divide");
            Console.WriteLine("5 - Square root");
            Console.WriteLine("6 - Qubed");
            Console.WriteLine("7 - Percentage");
            Console.WriteLine("8 - Exit");

            var rawInput = Console.ReadLine();

            if (int.TryParse(rawInput, out int functionInput))
            {
            }
            else
            {
                Console.WriteLine("Invalid input, please enter a valid integer.");
                continue;
            }
            double? result = null;

            if (functionInput == 1)
            {
                Console.WriteLine("How many numbers would you like to add?");
                var howManyAdd = int.Parse(Console.ReadLine());
                var numbersToAdd = GetNumbers(howManyAdd);
                result = AddNumbers(numbersToAdd);
            }
            else if (functionInput == 2)
            {
                Console.WriteLine("Enter the first number:");
                var sub1 = int.Parse(Console.ReadLine());
                Console.WriteLine("Enter the second number:");
                var sub2 = int.Parse(Console.ReadLine());
                result = sub1 - sub2;
            }
            else if (functionInput == 3)
            {
                Console.WriteLine("How many numbers would you like to multiply?");
                var howManyMult = int.Parse(Console.ReadLine());
                var numbersToMultiply = GetNumbers(howManyMult);
                result = MultiplyNumbers(numbersToMultiply);
            }
            else if (functionInput == 4)
            {
                Console.WriteLine("How many numbers would you like to divide?");
                var howManyDivide = int.Parse(Console.ReadLine());
                var numbersToDivide = GetNumbers(howManyDivide);
                result = DivideNumbers(numbersToDivide);
               
            }
            else if (functionInput == 5)
            {
                Console.WriteLine("Enter a number to find its square root:");
                var numberToRoot = int.Parse(Console.ReadLine());
                result = Math.Sqrt(numberToRoot);

                if (numberToRoot < 0)
                {
                    Console.WriteLine("The square root is negative please try again.");
                }
            }
            else if (functionInput == 6)
            {
                Console.WriteLine("Enter a number to find its power:");
                var numberToRoot = int.Parse(Console.ReadLine());
                result = Math.Pow(numberToRoot, 3);
            }
            else if (functionInput == 7)
            {
                Console.WriteLine("Enter a number to find its percentage: ");
                var number = int.Parse(Console.ReadLine());
                Console.WriteLine("what percentage do you want to find: ");
               var percentage = double.Parse(Console.ReadLine());

               var resultpercentage = (percentage / 100.0) * number;
               result = resultpercentage;
               
               Console.WriteLine(percentage + "% of " + number + " is " + result);
            }
            
            else if (functionInput == 8)
            {
                Console.WriteLine("Thank you for using the calculator!");
                shouldRun = false;
            }
            
            else
            {
                Console.WriteLine("Invalid choice. Please enter a number from 1 to 6.");
            }
          

            if (result != null)
            {
                Console.WriteLine("Result is: " + result);
            }
            Console.WriteLine("would you like to do anotother calculation? (yes/no)");
            
            string answer = Console.ReadLine().ToUpper();

            if (answer == "YES")
            {
                shouldRun = true;
            }
            else if (answer == "NO")
            {
                shouldRun = false;
            }
             
        }

        Console.WriteLine("Program has ended thanks for using. Goodbye!");
    }

    static List<int> GetNumbers(int howManyNumbers)
    {
        var result = new List<int>();
        for (int i = 0; i < howManyNumbers; i++)
        {
            Console.WriteLine("Please enter the " + (i + 1) + "th number:");
            var recordedNumber = int.Parse(Console.ReadLine()); 
            result.Add(recordedNumber);
        }

        return result;
    }

    public static int AddNumbers(List<int> numbers)
    {
        var result = 0;
        foreach (var number in numbers)      
        {
            result += number;
        }
        return result;
    }

    static int MultiplyNumbers(List<int> numbers)
    {
        var result = 1;
        foreach (var number in numbers)
        {
            result *= number;
        }
        return result;
    }

    static double DivideNumbers(List<int> numbers)
    {
        double result = numbers[0];

        for (int i = 1; i < numbers.Count; i++)
        {
            if (numbers[i] == 0)
            {
                Console.WriteLine("Error: Division by zero detected.");
                return 0;
            }
            result /= numbers[i];
        }

        return result;
    }
}
*/