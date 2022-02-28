﻿using Microsoft.AspNetCore;

namespace BlazorClientsideSample.Server;

public class Program
{
    public static void Main(string[] args)
    {
        BuildWebHost(args).Run();
    }

    public static IWebHost BuildWebHost(string[] args)
    {
        return WebHost.CreateDefaultBuilder(args)
            .UseConfiguration(new ConfigurationBuilder().AddCommandLine(args).Build())
            .UseStartup<Startup>()
            .Build();
    }
}