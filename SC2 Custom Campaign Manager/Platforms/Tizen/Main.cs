using System;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace SC2_Custom_Campaign_Manager;

class Program : MauiApplication
{
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    static void Main(string[] args)
    {
        var app = new Program();
        app.Run(args);
    }
}