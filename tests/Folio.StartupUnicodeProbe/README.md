# Startup Unicode integration probe

This opt-in Windows probe compiles the production task reader and backend with
Folio's actual UTF-8 application manifest. It checks Unicode task names and
paths, the registration contract, snapshot restoration, missing tasks, and both
STA and MTA callers against the real Task Scheduler.

The probe creates one disabled task with a unique `Folio Unicode Probe-` name
and removes it in `finally`. It never enables, overwrites, or deletes the user's
Folio startup task, and does not launch Folio. Run it as an ordinary user.

From the repository root:

```powershell
dotnet run --project .\tests\Folio.StartupUnicodeProbe\Folio.StartupUnicodeProbe.csproj
```

To check Native AOT, use an x64 Visual Studio developer shell:

```powershell
dotnet publish .\tests\Folio.StartupUnicodeProbe\Folio.StartupUnicodeProbe.csproj -c Release -p:Platform=x64 -r win-x64 -p:PublishAot=true -p:IlcUseEnvironmentalTools=true
& .\tests\Folio.StartupUnicodeProbe\bin\x64\Release\net10.0-windows\win-x64\publish\Folio.StartupUnicodeProbe.exe
```

Run the generated executable directly, so the process uses Folio's manifest.
The Native AOT executable can also be copied to a Windows 10 machine without
installing the .NET SDK. A successful probe does not replace a real login/startup
test of the application on that machine.
