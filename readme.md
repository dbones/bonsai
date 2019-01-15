## ![Bonsai](https://raw.githubusercontent.com/dbones/bonsai/master/images/bonsai-large.png "Bonsai IoC")


[![Build status](https://ci.appveyor.com/api/projects/status/wjj6iy88fdsl1sy7/branch/master?svg=true)](https://ci.appveyor.com/project/dbones/bonsai/branch/master) [![codecov](https://codecov.io/gh/dbones/bonsai/branch/master/graph/badge.svg)](https://codecov.io/gh/dbones/bonsai) ![](https://img.shields.io/nuget/v/Bonsai.Ioc.svg)


A small .NET IoC container, written in .NET Core 2.1.

Currently a work in progress. ( brand new :) )

# Features 

* Constructor injection
* Fluent Registration
* Open Generics
* Named services
* Scopes (Transient, Singleton, Named, Scoped)
* Tracks IDisposable instances
* more to come....


# Example usage

(note please look at the tests they show each feature in use)

Provide as many modules as you like, these allow you to register contracts with the IoC container.


```
class RegisterContracts : IModule
{
    public void Setup(ContainerBuilder builder)
    {
        builder.Register<ClassMonitor>().As<ClassMonitor>().Scoped<Singleton>();
        builder.Register<ServiceWithCtorAndDisposable>().As<IService>().Scoped<Singleton>();
        builder.Register<ServiceWithCtor>().As<IService>("simple").Scoped<Transient>();
        builder.Register<LoggerPlain>().As<ILogger>().Scoped<Transient>();
    }
}
```

Create the container and use it:

```
//setup the container
var builder = new ContainerBuilder();
builder.SetupModules(new RegisterContracts());

//create the container and then create any scopes you need.
using(var container = builder.Create())
using(var scope = container.CreateScope())
{ 
    var service = scope.Resolve<IService>();
}
```

## Performance - this is still a work in progress.

for example here is how Bonsai stacks up against some other IoC containers

Note: that all the **other** IoC **provide** far **more** features, at the moment, but this is still quite interesting to see

``` ini

BenchmarkDotNet=v0.11.3, OS=Windows 10.0.17134.523 (1803/April2018Update/Redstone4)
Intel Core i7-8550U CPU 1.80GHz (Kaby Lake R), 1 CPU, 8 logical and 4 physical cores
Frequency=1945311 Hz, Resolution=514.0566 ns, Timer=TSC
.NET Core SDK=2.1.403
  [Host]     : .NET Core 2.1.5 (CoreCLR 4.6.26919.02, CoreFX 4.6.26919.02), 64bit RyuJIT
  Job-RHODRM : .NET Core 2.1.5 (CoreCLR 4.6.26919.02, CoreFX 4.6.26919.02), 64bit RyuJIT

InvocationCount=5000  UnrollFactor=50  

```
|  Method |        Mean |      Error |     StdDev | Ratio | RatioSD | Rank | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
|-------- |------------:|-----------:|-----------:|------:|--------:|-----:|------------:|------------:|------------:|--------------------:|
|   Bonsai |   319.67 ns |  14.575 ns |  41.582 ns |  1.00 |    0.00 |   ** |           - |           - |           - |               538 B |
| Windsor | 9,144.42 ns | 177.181 ns | 472.931 ns | 28.39 |    3.68 | **** |      1.0000 |           - |           - |              4344 B |
| Autofac | 1,924.92 ns |  57.994 ns | 170.087 ns |  6.13 |    0.92 |  *** |      0.4000 |           - |           - |              2344 B |
|   Grace |    43.50 ns |   3.146 ns |   8.874 ns |  0.14 |    0.04 |    * |           - |           - |           - |               104 B |


Things we have tired or have yet to try:

* precomiling all delegates - done
* use a linked list instead of a stack - done
* remove dependency on the key dictionay (as much as possible) - done 
* replace dictionary with immutable AVL
* replace Concurancy with Out of the box concurancy collection
* look into interlock instead of the lightweigh read write lock