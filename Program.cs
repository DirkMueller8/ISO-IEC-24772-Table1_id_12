using System;

namespace Null
{
    // This example illustrates the ISO/IEC 24772-1 recommendation in Table 1, number 12:
    // "rohibit the modification of loop control variables inside the loop body.
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Problematic example (demonstrates prohibited modification of loop control variable):");
            ProblematicLoop();

            Console.WriteLine();
            Console.WriteLine("Corrected example (follows ISO/IEC 24772-1 recommendation):");
            try
            {
                SafeLoop();
            }
            catch (OperationCanceledException ex)
            {
                Console.WriteLine($"Input cancelled: {ex.Message}");
            }
        }

        // Problematic: modifies the loop control variable (`i`) inside the loop body.
        // This can cause off-by-one errors, index-out-of-range exceptions, or infinite loops.
        static void ProblematicLoop()
        {
            int[] matrix = new int[3];
            int i = 0;
            Console.WriteLine($"Matrix length: {matrix.Length}");

            while (i < matrix.Length)
            {
                Console.WriteLine("Give the value:");
                try
                {
                    // If the user enters invalid input, the catch decrements `i` to retry.
                    // Modifying `i` here violates the guideline and is brittle/unsafe.
                    matrix[i] = Convert.ToInt32(Console.ReadLine());
                }
                catch (Exception)
                {
                    Console.WriteLine("Conversion failed — modifying loop control variable (unsafe).");
                    i--; // <- problematic modification
                }
                i++;
            }

            i = 0;
            foreach (int item in matrix)
            {
                Console.WriteLine("The value in {0} is {1}.", i, item);
                i++;
            }
        }

        // Safe: never change the loop control variable inside the loop.
        // Use an inner validation loop or TryParse to handle input retries.
        // Added: null/empty input handling and a cancel token ("quit" or "cancel").
        static void SafeLoop()
        {
            int[] matrix = new int[3];

            for (int i = 0; i < matrix.Length; i++)
            {
                // Use a dedicated inner loop to validate input for the current index.
                while (true)
                {
                    Console.WriteLine("Give the value (or type 'quit' to cancel):");
                    string? raw = Console.ReadLine();

                    // Handle end-of-stream (null) explicitly
                    if (raw is null)
                    {
                        Console.WriteLine("No input (end-of-stream) detected — cancelling.");
                        throw new OperationCanceledException("End of input stream.");
                    }

                    string input = raw.Trim();

                    // Cancel token support
                    if (string.Equals(input, "quit", StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(input, "cancel", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new OperationCanceledException("User requested cancellation.");
                    }

                    // Empty input handling
                    if (string.IsNullOrEmpty(input))
                    {
                        Console.WriteLine("Empty input — please enter a number or type 'quit' to cancel.");
                        continue;
                    }

                    // Try parse and accept valid integers
                    if (int.TryParse(input, out int value))
                    {
                        matrix[i] = value;
                        break; // valid input: exit validation loop and let the for-loop increment i
                    }

                    Console.WriteLine("Conversion failed — please try again or type 'quit' to cancel.");
                }
            }

            for (int i = 0; i < matrix.Length; i++)
            {
                Console.WriteLine("The value in {0} is {1}.", i, matrix[i]);
            }
        }
    }
}