# Lyrics Fetcher

A C# console application that fetches song lyrics from a lyrics API.

## Features
- Get lyrics for any artist and track  
- Cache lyrics locally in JSON format  
- Load lyrics from cache to avoid repeated API calls  
- Export lyrics to plain text files  

## Setup

1. **Get an API Key**  
   - Sign up at your chosen lyrics API provider  
   - Get your free API key  

2. **Configure the Application**  
   - Create an appsettings.json  

3. **Build the Application**  
   ```bash
   dotnet build MySolution.slnx
   ```
4. **Run the Application**  
   ```bash
   dotnet run
   ```

## Usage
- Enter an artist and track name  
- View lyrics fetched from API or loaded from cache  
- Optionally export lyrics to a plain text file  
