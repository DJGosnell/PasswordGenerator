using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace PasswordGenerator;

/// <summary>
/// Generates passwords by combining a specified number of words (from a word list), random numbers,
/// an optional special character, and optional uppercase transformation of one word.
/// Parsing is separated into its own method so you can load once and call multiple times.
/// Adjacent digits are always squashed (no space) regardless of spacing settings.
/// </summary>
public class Generator
{
    public int MaxReadLines { get; set; }
    public int NumberOfWords { get; set; }
    public int NumberOfNumbers { get; set; }
    public bool AllowSpacesBetweenWordsAndNumbers { get; set; }

    /// <summary>
    /// If true, include one special character from the provided list.
    /// </summary>
    public bool IncludeSpecialCharacter { get; set; }

    /// <summary>
    /// List of possible special characters to use (e.g. '!','@','#').
    /// </summary>
    public IList<char> SpecialCharacters { get; set; }

    /// <summary>
    /// If true, randomly uppercase one of the selected words.
    /// </summary>
    public bool UppercaseFirstLetterOfOneWord { get; set; }

    private readonly List<string> _words;
    private static readonly RandomNumberGenerator rng = RandomNumberGenerator.Create();

    /// <summary>
    /// Initializes a new instance of the <see cref="Generator"/> class.
    /// </summary>
    public Generator(
        int maxReadLines,
        int numberOfWords,
        int numberOfNumbers,
        bool allowSpaces,
        bool includeSpecialCharacter,
        IEnumerable<char>? specialCharacters,
        bool uppercaseFirstLetterOfOneWord)
    {
        MaxReadLines = maxReadLines;
        NumberOfWords = numberOfWords;
        NumberOfNumbers = numberOfNumbers;
        AllowSpacesBetweenWordsAndNumbers = allowSpaces;
        IncludeSpecialCharacter = includeSpecialCharacter;
        SpecialCharacters = specialCharacters != null
            ? new List<char>(specialCharacters)
            : new List<char>();
        UppercaseFirstLetterOfOneWord = uppercaseFirstLetterOfOneWord;

        _words = new List<string>(maxReadLines);
    }

    /// <summary>
    /// Parses the words file up to <see cref="MaxReadLines"/> lines.
    /// Call this once before generating passwords to load the word list into memory.
    /// </summary>
    public void ParseWords()
    {
        using var stream = typeof(Generator).Assembly.GetManifestResourceStream("PasswordGenerator.words.txt");
        _words.Clear();
        using (var reader = new StreamReader(stream!))
        {
            string? line;
            int count = 0;
            while (count < MaxReadLines && (line = reader.ReadLine()) != null)
            {
                _words.Add(line);
                count++;
            }
        }

        if (_words.Count < NumberOfWords)
            throw new InvalidOperationException(
                $"Not enough words in word resource. (read {_words.Count}, required {NumberOfWords}).");
    }

    /// <summary>
    /// Generates a password using the pre-parsed words and optional features.
    /// </summary>
    /// <returns>A single-line password string.</returns>
    public string GeneratePassword()
    {
        if (_words.Count == 0)
            throw new InvalidOperationException("Word list not loaded. Call ParseWords() before generating passwords.");

        // Select random words
        var selectedWords = new List<string>(NumberOfWords);
        for (int i = 0; i < NumberOfWords; i++)
        {
            int idx = GetRandomInt(0, _words.Count);
            selectedWords.Add(_words[idx]);
        }

        // Optionally uppercase one word
        if (UppercaseFirstLetterOfOneWord && selectedWords.Count > 0)
        {
            int wordToUpper = GetRandomInt(0, selectedWords.Count);
            var word = selectedWords[wordToUpper];
            if(word.Length == 1)
                selectedWords[wordToUpper] = $"{char.ToUpperInvariant(word[0])}";
            else 
                selectedWords[wordToUpper] = $"{char.ToUpperInvariant(word[0])}{string.Join("", word.Skip(1))}";
            
        }

        // Merge into tokens list
        var tokens = new List<string>(NumberOfWords + NumberOfNumbers + (IncludeSpecialCharacter ? 1 : 0));
        tokens.AddRange(selectedWords);

        // Insert random digits at random positions (not at beginning)
        for (int i = 0; i < NumberOfNumbers; i++)
        {
            string digit = ((char)('0' + GetRandomInt(0, 10))).ToString();
            int pos = GetRandomInt(1, tokens.Count + 1);
            tokens.Insert(pos, digit);
        }

        // Optionally insert one special character
        if (IncludeSpecialCharacter && SpecialCharacters.Count > 0)
        {
            char special = SpecialCharacters[GetRandomInt(0, SpecialCharacters.Count)];
            int pos = GetRandomInt(1, tokens.Count + 1);
            tokens.Insert(pos, special.ToString());
        }

        // Build password string: avoid spaces between adjacent digits
        var sb = new StringBuilder();
        if (tokens.Count > 0)
        {
            sb.Append(tokens[0]);
            bool prevIsDigit = tokens[0].Length == 1 && char.IsDigit(tokens[0][0]);

            for (int i = 1; i < tokens.Count; i++)
            {
                string current = tokens[i];
                bool currIsDigit = current.Length == 1 && char.IsDigit(current[0]);

                if (AllowSpacesBetweenWordsAndNumbers && !(prevIsDigit && currIsDigit))
                {
                    sb.Append(' ');
                }

                sb.Append(current);
                prevIsDigit = currIsDigit;
            }
        }

        return sb.ToString();
    }
    
    /// <summary>
    /// Generates a password using the pre-parsed words.
    /// </summary>
    public string GeneratePassword2()
    {
        if (_words.Count == 0)
            throw new InvalidOperationException("Word list not loaded. Call ParseWords() before generating passwords.");

        // Select random words
        var selected = new List<string>(NumberOfWords);
        for (int i = 0; i < NumberOfWords; i++)
        {
            int idx = GetRandomInt(0, _words.Count);
            selected.Add(_words[idx]);
        }
        
        // Optionally uppercase first letter of one word
        if (UppercaseFirstLetterOfOneWord && selected.Count > 0)
        {
            int wordToUpper = GetRandomInt(0, selected.Count);
            var word = selected[wordToUpper];
            if(word.Length == 1)
                selected[wordToUpper] = $"{char.ToUpperInvariant(word[0])}";
            else 
                selected[wordToUpper] = $"{char.ToUpperInvariant(word[0])}{string.Join("", word.Skip(1))}";
            
        }

        // Build with separators between each word
        var sb = new StringBuilder();
        sb.Append(selected[0]);
        for (int i = 1; i < selected.Count; i++)
        {
            string sep;
            bool useSpecial = IncludeSpecialCharacter && SpecialCharacters.Count > 0 && GetRandomInt(0, 2) == 0;
            if (useSpecial)
            {
                sep = SpecialCharacters[GetRandomInt(0, SpecialCharacters.Count)].ToString();
            }
            else
            {
                int digitCount = GetRandomInt(1, 3); // 1 or 2 digits
                var dsb = new StringBuilder(digitCount);
                for (int d = 0; d < digitCount; d++)
                    dsb.Append((char)('0' + GetRandomInt(0, 10)));
                sep = dsb.ToString();
            }
            sb.Append(sep);
            sb.Append(selected[i]);
        }

        return sb.ToString();
    }

    /// <summary>
    /// Returns a random integer in the range [minValue, maxValue).
    /// </summary>
    private static int GetRandomInt(int minValue, int maxValue)
    {
        if (minValue >= maxValue)
            throw new ArgumentException("minValue must be less than maxValue");

        long range = (long)maxValue - minValue;
        var buffer = new byte[4];
        rng.GetBytes(buffer);
        uint rand = BitConverter.ToUInt32(buffer, 0);
        return (int)(rand % range) + minValue;
    }
}