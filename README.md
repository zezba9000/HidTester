# HidTester
* HidSharp directly included as source for better debugging
* Run as Admin/Root
* Defaults to MSI-Claw gamepad if no args set
```
-vid={dec|hex}
-pid={dec|hex}
-read-delay={ms}
-data-file={text-file-path} [each byte on its own line]
```

### Windows .NET Publishing
* AOT: dotnet publish -r win-x64 -c Release /p:PublishAot=true
* Jit: dotnet publish -r win-x64 -c Release

### macOS .NET Publishing
* AOT: dotnet publish -r osx-x64|osx-arm64 -c Release /p:PublishAot=true
* Jit: dotnet publish -r osx-x64|osx-arm64 -c Release

### Linux .NET Publishing
* AOT: dotnet publish -r linux-x64 -c Release /p:PublishAot=true
* Jit: dotnet publish -r linux-x64 -c Release