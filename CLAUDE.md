# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

This is a .NET 9.0 console application that generates secure passwords by combining random words from an embedded word list with numbers and special characters. The application uses cryptographically secure random number generation and provides interactive configuration options.

## Common Commands

### Build and Run
```bash
# Build the solution
dotnet build src/PasswordGenerator.sln

# Run the application
dotnet run --project src/PasswordGenerator/PasswordGenerator.csproj

# Build optimized release version
dotnet build src/PasswordGenerator.sln -c Release

# Publish as single-file executable (configured for win-x64)
dotnet publish src/PasswordGenerator/PasswordGenerator.csproj -c Release
```

### Development
```bash
# Restore NuGet packages
dotnet restore src/PasswordGenerator.sln

# Clean build artifacts
dotnet clean src/PasswordGenerator.sln
```

## Architecture

The application consists of three main components:

### Core Classes
- **`Generator`** (`PasswordGenerator.cs:17`): Main password generation engine that loads word lists from embedded resources and provides two password generation algorithms
- **`InputHelper`** (`InputHelper.cs:6`): Generic input validation utility that handles typed console input with default values and ESC cancellation
- **`Program`** (`Program.cs:5`): Entry point that handles user interaction and orchestrates password generation

### Key Design Elements
- **Embedded Word List**: The `words.txt` file is embedded as a resource and loaded into memory during initialization
- **Cryptographic Security**: Uses `RandomNumberGenerator` for secure random number generation rather than `Random`
- **Two Generation Algorithms**: 
  - `GeneratePassword()`: Places elements randomly with configurable spacing
  - `GeneratePassword2()`: Uses separators between words (current default)
- **Interactive Configuration**: Users configure word complexity (1-18 levels), number of words/numbers, spacing, special characters, and uppercase options

### Resource Management
- Word list is loaded once via `ParseWords()` method and reused for multiple password generations
- Supports configurable complexity levels that determine how many words to read from the embedded word list (up to 172,782 words)
- Memory-efficient design loads only the required number of words based on complexity setting

### Build Configuration
The project is configured for single-file deployment with aggressive trimming enabled for minimal distribution size on Windows x64.