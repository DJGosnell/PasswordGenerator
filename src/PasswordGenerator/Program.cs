using System;
using System.Diagnostics;
using System.IO;
using PasswordGenerator;

Console.WriteLine("Password Generator");

int? levelOfWords = 2;

if (!InputHelper.PromptInput("Mode (1 = Spaces in Password, 2 = No Spaces)", 2, out var mode)
    || !InputHelper.PromptInput("Complexity of words used (1 least complex - 18 most complex)", levelOfWords.Value, out levelOfWords))
    return;

bool? specialCharacter = true;
bool? uppercaseOneWord = true;
int? words = 10;
int? numbers = 5;
string? outputFilePath = null;
if (mode.Value == 1)
{
    if (!InputHelper.PromptInput("Use one special character", specialCharacter.Value, out specialCharacter)
        || !InputHelper.PromptInput("Set one word as uppercase", uppercaseOneWord.Value, out uppercaseOneWord)
        || !InputHelper.PromptInput("Number of words", words.Value, out words)
        || !InputHelper.PromptInput("Number of numbers", numbers.Value, out numbers))
        return;
    
    if (!InputHelper.PromptInput("Output to file (leave empty for console only)", string.Empty, out outputFilePath))
        return;
    
    if (!string.IsNullOrWhiteSpace(outputFilePath))
    {
        try
        {
            var directory = Path.GetDirectoryName(outputFilePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating directory: {ex.Message}");
            return;
        }
    }
}

var sw = Stopwatch.StartNew();
Console.Write("Parsing Words... ");
var wordLines = Math.Min(172681, levelOfWords.Value * 10000);
char[] specialCharacters = ['~','!','@','#','$','%','^','&','*'];
var generator = new Generator(wordLines, words.Value, numbers.Value, mode.Value == 1, specialCharacter.Value, specialCharacters, uppercaseOneWord.Value);
generator.ParseWords();
Console.WriteLine($"Completed in {sw.ElapsedMilliseconds}ms.");

while (true)
{
    if (!InputHelper.PromptInput("Enter number of passwords to generate", 10, out var passwordIterations))
        return;
    sw.Restart();
    
    StreamWriter? fileWriter = null;
    try
    {
        if (mode.Value == 1 && !string.IsNullOrWhiteSpace(outputFilePath))
        {
            fileWriter = new StreamWriter(outputFilePath, append: false);
            Console.WriteLine($"Writing passwords to: {outputFilePath}");
        }
        
        bool cancelled = false;
        for (int i = 0; i < passwordIterations && !cancelled; i++)
        {
            // Check for escape key press more frequently
            while (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Escape)
                {
                    cancelled = true;
                    Console.WriteLine($"\r\nOperation cancelled. Wrote {i} of {passwordIterations} passwords to file.");
                    break;
                }
            }
            
            if (cancelled) break;
            
            var password = mode.Value == 2 ? generator.GeneratePassword2() : generator.GeneratePassword();
            
            if (fileWriter != null)
            {
                fileWriter.WriteLine(password);
                
                // Update progress bar every 10 passwords or on the last password for more responsive cancellation
                if ((i + 1) % 10 == 0 || i == passwordIterations - 1)
                {
                    var progress = (double)(i + 1) / passwordIterations;
                    var barWidth = 50;
                    var filledWidth = (int)(progress * barWidth);
                    var progressBar = new string('█', filledWidth) + new string('░', barWidth - filledWidth);
                    Console.Write($"\r[{progressBar}] {i + 1}/{passwordIterations} ({progress:P0}) - Press ESC to cancel");
                }
            }
            else
            {
                Console.WriteLine(password);
            }
        }
        
        if (fileWriter != null && !cancelled)
        {
            Console.WriteLine($"\nSuccessfully wrote {passwordIterations} passwords to {outputFilePath}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"\nError writing to file: {ex.Message}");
    }
    finally
    {
        fileWriter?.Dispose();
    }

    Console.WriteLine();
    Console.WriteLine($"Generated {passwordIterations} passwords in {sw.ElapsedMilliseconds}ms.");
}