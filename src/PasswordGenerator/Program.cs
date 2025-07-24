using System;
using System.Diagnostics;
using PasswordGenerator;

Console.WriteLine("Password Generator");

if (!InputHelper.PromptInput("Mode (1 = Spaces in Password, 2 = No Spaces)", 2, out var mode))
    return;

bool? specialCharacter = true;
bool? uppercaseOneWord = true;
int? levelOfWords = 2;
int? words = 3;
int? numbers = 2;
if (mode.Value == 1)
{
    if (!InputHelper.PromptInput("Use one special character", specialCharacter.Value, out specialCharacter)
        || !InputHelper.PromptInput("Set one word as uppercase", uppercaseOneWord.Value, out uppercaseOneWord)
        || !InputHelper.PromptInput("Complexity of words used (1 least complex - 18 most complex)", levelOfWords.Value, out levelOfWords)
        || !InputHelper.PromptInput("Number of words", words.Value, out words)
        || !InputHelper.PromptInput("Number of numbers", numbers.Value, out numbers))
        return;
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
    for (int i = 0; i < passwordIterations; i++)
    {
        Console.WriteLine(mode.Value == 2 ? generator.GeneratePassword2() : generator.GeneratePassword());
    }

    Console.WriteLine();
    Console.WriteLine($"Generated {passwordIterations} passwords in {sw.ElapsedMilliseconds}ms.");
}