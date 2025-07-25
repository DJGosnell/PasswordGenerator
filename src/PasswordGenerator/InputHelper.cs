using System;
using System.Diagnostics.CodeAnalysis;

namespace PasswordGenerator;

public class InputHelper
{

    public static bool PromptInput<T>(string prompt, T defaultValue, [NotNullWhen(true)] out T? result) where T : struct
    {
        result = null;

        while (true)
        {
            Console.Write($"{prompt} (Default: {defaultValue}, ESC to cancel): ");
            string input = ReadLineWithEscape(out bool escaped);

            if (escaped)
                return false;

            if (string.IsNullOrWhiteSpace(input))
            {
                result = defaultValue;
                return true;
            }

            try
            {
                if (typeof(T) == typeof(int))
                    result = (T)(object)int.Parse(input);
                else if (typeof(T) == typeof(float))
                    result = (T)(object)float.Parse(input);
                else if (typeof(T) == typeof(double))
                    result = (T)(object)double.Parse(input);
                else if (typeof(T) == typeof(bool))
                    result = (T)(object)bool.Parse(input);
                else if (typeof(T) == typeof(decimal))
                    result = (T)(object)decimal.Parse(input);
                else
                    throw new NotSupportedException($"Type {typeof(T).Name} is not supported.");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Invalid input: {ex.Message}. Please try again.");
            }
        }
    }

    public static bool PromptInput<T>(string prompt, out T? result) where T : struct
    {
        result = null;

        while (true)
        {
            Console.Write($"{prompt} (Press ESC to cancel): ");
            string input = ReadLineWithEscape(out bool escaped);

            if (escaped)
                return false;

            try
            {
                if (typeof(T) == typeof(int))
                    result = (T)(object)int.Parse(input);
                else if (typeof(T) == typeof(float))
                    result = (T)(object)float.Parse(input);
                else if (typeof(T) == typeof(double))
                    result = (T)(object)double.Parse(input);
                else if (typeof(T) == typeof(bool))
                    result = (T)(object)bool.Parse(input);
                else if (typeof(T) == typeof(decimal))
                    result = (T)(object)decimal.Parse(input);
                else
                    throw new NotSupportedException($"Type {typeof(T).Name} is not supported.");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Invalid input: {ex.Message}. Please try again.");
            }
        }
    }

    public static bool PromptInput(string prompt, string defaultValue, out string? result)
    {
        result = null;

        Console.Write($"{prompt} (Default: {defaultValue}, ESC to cancel): ");
        string input = ReadLineWithEscape(out bool escaped);

        if (escaped)
            return false;

        result = string.IsNullOrWhiteSpace(input) ? defaultValue : input;
        return true;
    }

    public static bool PromptInput(string prompt, out string? result)
    {
        result = null;

        Console.Write($"{prompt} (Press ESC to cancel): ");
        string input = ReadLineWithEscape(out bool escaped);

        if (escaped)
            return false;

        result = input;
        return true;
    }

    private static string ReadLineWithEscape(out bool escaped)
    {
        escaped = false;
        string input = string.Empty;

        while (true)
        {
            var key = Console.ReadKey(intercept: true);

            if (key.Key == ConsoleKey.Escape)
            {
                escaped = true;
                Console.WriteLine();
                return string.Empty;
            }

            if (key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                return input;
            }

            if (key.Key == ConsoleKey.Backspace && input.Length > 0)
            {
                input = input.Substring(0, input.Length - 1);
                Console.Write("\b \b");
            }
            else if (!char.IsControl(key.KeyChar))
            {
                input += key.KeyChar;
                Console.Write(key.KeyChar);
            }
        }
    }
}