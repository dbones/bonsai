﻿namespace Bones.Benchmarks
{
    using BenchmarkDotNet.Running;

    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<TransientBench>();
            BenchmarkRunner.Run<SingletonBench>();
            BenchmarkRunner.Run<ScopeBench>();
            BenchmarkRunner.Run<MixedBench>();
            //BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
        }
    }
}