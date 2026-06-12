# StarCraft II Custom Campaign Mod Manager

This is a work-in-progress cross-platform custom campaign mod manager for StarCraft II based on the awesome work of the GiantGrantGames community.
GiantGrantGames and his community have come up with a custom campaign mod manager that allows modders to use the standard campaign UI for custom campaigns.
They have developed and freely released their mod manager, however their mod manager only works on Windows.

This mod manager aims to work on both Mac and Windows as both platforms are officially supported by Blizzard for StarCraft II.

Currently only Mac on Intel/ARM processors and Windows 10 and 11 have been tested and verified.

## Fork Notes (ARM Mac Support)
This fork adds support for ARM Macs while leaving as much unchanged as possible. The following changes have been made from the original
* Updated target framework to .NET 10 for Mac builds
* Increased minimum MacOS version to 15.0 Sequoia. This is enforced by the newer .NET Mac Catalyst SDK
* Made minor code changes for updating to newer .NET API
* An Intel Mac build is provided but untested

## MAUI UI

Tested on Mac and Windows. Published for Mac, haven't figured out publishing for Windows. WinForm is used for the published Windows version.

### Build Requirements
#### Windows
The simplest setup is to install Visual Studio 2022 and add Android, iOS, C#, and MAUI to your Visual Studio.
However, if you don't want to install Visual Studio, then you can install just the .NET CLI and go from there.

#### Mac
> **Note:** Visual Studio for Mac has been discontinued. Use the .NET CLI instead.

* Go to https://dotnet.microsoft.com/en-us/download and install **.NET 10**
* Run the following commands:
    * `sudo dotnet workload install maui`
* Once that finishes, run:
    * `dotnet run --project "SC2 Custom Campaign Manager" -f net10.0-maccatalyst`

> **Note for Mac users:** Always specify `-f net10.0-maccatalyst` explicitly in your build commands. The solution contains a Windows-only project (`SC2_CCM_WinForm`) which will cause errors if you attempt to build the whole solution without specifying a target framework.

## Building

### Mac
To build, run `dotnet run --project "SC2 Custom Campaign Manager" -f:net10.0-maccatalyst -c:Debug`

To create a pkg file, run `dotnet build "SC2 Custom Campaign Manager" -f:net10.0-maccatalyst -c:Debug /p:CreatePackage=true`

#### Local publishing

* Run `dotnet publish "SC2 Custom Campaign Manager" -f:net10.0-maccatalyst -c:Debug -o out`

### Windows
To build, run `dotnet build "SC2 Custom Campaign Manager" -f:net6.0-windows -c:Release`

## WinForm

The WinForm UI has only been tested with Windows. The easiest way to build it is to load the solution in Visual Studio and then run the WinForm project.
This is what's published for Windows

### Publishing

I use Visual Studio's publishing feature for this project. Publishing is currently for the "folder publish" type and "folder publish" location. My settings are as follows:

* 64-bit Settings
  * Configuration: Release | Any CPU
  * Target framework: net6.0-windows
  * Deployment mode: Self-contained
  * Target runtime: win-x64
  * Target location: bin\Release\net6.0-windows\publish\win-x64\
  * File publish options
    * Produce single file: Checked
    * Enable ReadyToRun compilation: Checked
* 32-bit Settings
  * Configuration: Release | Any CPU
  * Target framework: net6.0-windows
  * Deployment mode: Self-contained
  * Target runtime: win-x86
  * Target location: bin\Release\net6.0-windows\publish\win-x86\
  * File publish options
    * Produce single file: Checked
    * Enable ReadyToRun compilation: Checked

## Code Modularity

All of the core logic, logging, etc is in the Commons library. This means that the UI can easily be changed out as needed per platform. Originally, there were two UIs, but I decided to just move to one UI for now. However, in the future if someone wanted to create another UI on-top, it would be very simple.

The other advantage of this modularity is the UI code is both small and easy to read, so learning how to adapt new UIs should be fairly straightforward.

UI code is responsible for the following:
* Overriding where logging goes and doing any log rotation (if needed)
    * This allows for UIs to choose an "internet connected" log reporting, local logging, no logging, etc
* Defining a "ShowMessage" method which takes a string and (hopefully) shows the user a message in some sort of UI logging
* Initializing an instance of SC2CCM
* Presenting the current UI state to the user (state is read from SC2CCM)
* Setting up UI handlers to forward events to SC2CCM

That's pretty much it. All of the unzipping, detecting installed campaigns, loading configuration, log messages, etc. is handled by the commons module. Manually written UI code should be around 300 lines of code, including class/method boilerplate and comments

## Features

* Import and load different campaigns
* Remembers last loaded campaign between sessions
* Allows toggling custom campaigns
* Displays campaign descriptions

### Lacking (not done yet)

* Drag and drop for imports

## Credits
Original SC2CCM by 7thAce - https://github.com/7thAce/SC2CCM  

SC2CCM Multi-Platform by matthewtolman - https://github.com/matthewtolman/sc2-custom-mod-manager  

ARM Mac update by Swagdude7 - https://github.com/Swagdude7/sc2-custom-mod-manager-ARM-update  