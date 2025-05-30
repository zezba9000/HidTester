# HidTester
* HidSharp directly included as source for better debugging

### Windows .NET Publishing
* AOT: dotnet publish -r win-x64 -c Release /p:PublishAot=true
* Jit: dotnet publish -r win-x64 -c Release

### macOS .NET Publishing
* AOT: dotnet publish -r osx-x64|osx-arm64 -c Release /p:PublishAot=true
* Jit: dotnet publish -r osx-x64|osx-arm64 -c Release

### Linux .NET Publishing
* AOT: dotnet publish -r linux-x64 -c Release /p:PublishAot=true
* Jit: dotnet publish -r linux-x64 -c Release