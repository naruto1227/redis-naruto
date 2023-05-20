// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using BenchmarkTest;
using Microsoft.Extensions.Logging;

Console.WriteLine("Hello, World!");

#if DEBUG
BenchmarkRunner.Run<StringGetTest>(new DebugInProcessConfig());
#else

var config = new ManualConfig().WithOptions(ConfigOptions.DisableOptimizationsValidator)
    .AddLogger(new ConsoleLogger());
config.AddDiagnoser(MemoryDiagnoser.Default);
config.AddColumnProvider(DefaultColumnProviders.Instance);
BenchmarkRunner.Run<StringGetTest>(config);
#endif


Console.ReadLine();