# StarCraft II Custom Campaign Mod Manager

This is a work-in-progress cross-platform custom campaign mod manager for StarCraft II based on the awesome work of the GiantGrantGames community.
GiantGrantGames and his community have come up with a custom campaign mod manager that allows modders to use the standard campaign UI for custom campaigns.
They have developed and freely released their mod manager, however their mod manager only works on Windows.

This mod manager aims to work on both Mac and Windows as both platforms are officially supported by Blizzard for StarCraft II.

Currently only Mac on Intel processors has been tested and verified.

## MAUI UI


### Build Requirements

For Windows, the simplest setup is to install Visual Studio 2022 and add Android, iOS, C#, and MAUI to your Visual Studio.
Howeer, if you don't want to install Visual Studio, then you can install just the .NET CLI and go from there.
For Mac users, you'll want to install install the .NET CLI (Visual Studio 2022 for Mac is still in pre-release).

This project requires .NET 6 to be installed as well as the MAUI workload.
To get everything setup, do the following:

* Go to https://dotnet.microsoft.com/en-us/download and install .NET 6
* Run the following command:
    * Windows: `dotnet workload install maui` (may need to run your powershell/cmd as an Administrator)
    * Mac: `sudo dotnet workload install maui`
* Once that finishes, run `dotnet run --project "SC2 Custom Campaign Manager"` to launch the program

## Building

### Mac
To build, run `dotnet run --project "SC2 Custom Campaign Manager" -f:net6.0-maccatalyst -c:Debug`

To create a pkg file, run `dotnet build "SC2 Custom Campaign Manager" -f:net6.0-maccatalyst -c:Debug /p:CreatePackage=true`

#### Local publishing

* Run `dotnet publish "SC2 Custom Campaign Manager" -f:net6.0-maccatalyst -c:Debug -o out`

### Windows
To build, run `dotnet build "SC2 Custom Campaign Manager" -f:net6.0-windows -c:Release`

## Avalonia UI

### Build Requirements

The simplest setup is to install Visual Studio and add C# (make sure you're version of visual studio supports .NET 6).
However, if you don't want to install Visual Studio, then you can install just the .NET CLI and go from there.

This project requires .NET 6 to be installed.
To get everything setup, do the following:

* Go to https://dotnet.microsoft.com/en-us/download and install .NET 6
* Once that finishes, run `dotnet run --project SC2_Avalonia_UI -c:Release` to launch the program

## Building

### Mac
To build, run `dotnet run --project SC2_Avalonia_UI -r:osx-x64 -c:Release`

To create a pkg file, run `dotnet build SC2_Avalonia_UI -r:osx-x64 -c:Release`

#### Local publishing

* Run `rm -rf out/avalon && dotnet restore -r osx-x64 && dotnet publish SC2_Avalonia_UI -c:Release -o out/avalon --sc --nologo -r:osx-x64 -p:PublishSingleFile=true`

#### Packaging

### Windows
To build, run `dotnet build SC2_Avalonia_UI -f:net6.0-windows -c:Release`

## Features

* Import and load different campaigns
* Remembers last loaded campaign between sessions
* Allows toggling cutom campaigns
* Displays campaign descriptions

### Lacking (not done yet)

* Drag and drop for imports

