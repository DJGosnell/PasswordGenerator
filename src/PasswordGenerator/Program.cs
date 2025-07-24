using System;
using System.Diagnostics;
using PasswordGenerator;

Console.WriteLine("Password Generator");

if (!InputHelper.PromptInput("Mode (1 = Spaces in Password, 2 = No Spaces)", 2, out var mode)
    || !InputHelper.PromptInput("Use one special character", true, out var specialCharacter)
    || !InputHelper.PromptInput("Set one word as uppercase", true, out var uppercaseOneWord)
    || !InputHelper.PromptInput("Complexity of words used (1 least complex - 18 most complex)", 2, out var levelOfWords)
    || !InputHelper.PromptInput("Spaces between words", true, out var spacesBetweenWords)
    || !InputHelper.PromptInput("Number of words", 3, out var words)
    ||!InputHelper.PromptInput("Number of numbers", 2, out var numbers))
    return;

var sw = Stopwatch.StartNew();
Console.Write("Parsing Words... ");
var wordLines = Math.Min(172681, levelOfWords.Value * 10000);
char[] specialCharacters = ['~','!','@','#','$','%','^','&','*'];
var generator = new Generator(wordLines, words.Value, numbers.Value, spacesBetweenWords.Value, specialCharacter.Value, specialCharacters, uppercaseOneWord.Value);
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