# Password Generator

A secure .NET console application that generates strong, memorable passwords by combining random words from a comprehensive word list with numbers and special characters.

## Features

- **Cryptographically Secure**: Uses `RandomNumberGenerator` for true randomness
- **Configurable Complexity**: Choose from 18 different word complexity levels (1-180,000~ words)
- **Flexible Formatting**: Two generation modes with different spacing and separator options
- **Interactive Configuration**: Easy-to-use prompts with sensible defaults
- **High Performance**: Optimized for generating multiple passwords quickly
- **Portable**: Single-file executable with no external dependencies

## Building

### Prerequisites
- .NET 9.0 SDK or later

### Build Commands

```bash
# Clone and navigate to project
cd src

# Restore dependencies
dotnet restore PasswordGenerator.sln

# Build debug version
dotnet build PasswordGenerator.sln

# Build release version
dotnet build PasswordGenerator.sln -c Release

# Create single-file executable
dotnet publish PasswordGenerator/PasswordGenerator.csproj -c Release
```

The published executable will be located in `src/PasswordGenerator/bin/Release/net9.0/win-x64/publish/`

## Usage

### Running the Application

```bash
# Run from source
dotnet run --project PasswordGenerator/PasswordGenerator.csproj

# Or run the published executable
.PasswordGenerator.exe
```

### Configuration Options

The application prompts for the following settings:

- **Mode**: Choose between spaced (1) or compact (2) password formats
- **Special Character**: Include one random special character (`~!@#$%^&*`)
- **Uppercase Word**: Randomly capitalize the first letter of one word
- **Word Complexity**: Level 1-18 (determines word list size: level × 10,000 words)
- **Spaces Between Words**: Add spaces between password components (mode 1 only)
- **Number of Words**: How many words to include in each password
- **Number of Numbers**: How many random digits to include

### Generation Modes

**Mode 1 (Spaced)**: Words and numbers are placed randomly with optional spacing
```
example: "secure 7 password 3 generation"
```

**Mode 2 (Compact)**: Words are separated by numbers or special characters
```
example: "secure7password3generation"
```

### Performance
- Memory efficient: Only loads required words based on complexity level

## Architecture

- **Program.cs**: Interactive console interface and main application flow
- **Generator.cs**: Core password generation algorithms and word list management
- **InputHelper.cs**: Generic typed input validation with escape handling
- **words.txt**: Embedded word list resource (172,782 words)

## Security Features
- Uses cryptographically secure random number generation
- No predictable patterns in password structure
- Configurable complexity levels to match security requirements
- Option to include special characters and mixed case