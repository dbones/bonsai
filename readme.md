## ![Bonsai](https://raw.githubusercontent.com/dbones/bonsai/master/images/bonsai-large.png "Bonsai IoC")


[![Nuget](https://img.shields.io/nuget/v/Bonsai.Ioc.svg)](https://www.nuget.org/packages/Bonsai.Ioc/)  [![Build status](https://ci.appveyor.com/api/projects/status/wjj6iy88fdsl1sy7/branch/master?svg=true)](https://ci.appveyor.com/project/dbones/bonsai/branch/master) [![codecov](https://codecov.io/gh/dbones/bonsai/branch/master/graph/badge.svg)](https://codecov.io/gh/dbones/bonsai) [![Codacy Badge](https://api.codacy.com/project/badge/Grade/fc52ced9d453411283c76179e1eb491a)](https://www.codacy.com/app/dbones/bonsai?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=dbones/bonsai&amp;utm_campaign=Badge_Grade)


A small .NET IoC container, written in .NET Standard 2.

# Features 

* Constructor injection
* Fluent Registration
* Open Generics
* Named services
* Scopes (Transient, Singleton, Named, Scoped)
* Tracks IDisposable instances
* IEnumerable
* Lazy
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

## Internal design

Initially Bonsai was designed with the ability to pre-compile all delegates including all generic types. 

However some libraries make use of the IoC container directly inside factory classes. this lead to the Planning namespace, which we want to refactor.


## Performance - this is still a work in progress.

for example here is how Bonsai stacks up against some other IoC containers

Note: that all the **other** IoC **provide** far **more** features, at the moment, but this is still quite interesting to see

``` ini

BenchmarkDotNet=v0.11.3, OS=Windows 10.0.17134.706 (1803/April2018Update/Redstone4)
Intel Core i7-8550U CPU 1.80GHz (Kaby Lake R), 1 CPU, 8 logical and 4 physical cores
Frequency=1945310 Hz, Resolution=514.0569 ns, Timer=TSC
.NET Core SDK=2.1.403
  [Host]     : .NET Core 2.1.5 (CoreCLR 4.6.26919.02, CoreFX 4.6.26919.02), 64bit RyuJIT
  Job-NMUDBD : .NET Core 2.1.5 (CoreCLR 4.6.26919.02, CoreFX 4.6.26919.02), 64bit RyuJIT

InvocationCount=5000  UnrollFactor=50  

```
|  Method |         Mean |      Error |      StdDev |       Median | Ratio | RatioSD | Rank | Gen 0/1k Op | Gen 1/1k Op | Gen 2/1k Op | Allocated Memory/Op |
|-------- |-------------:|-----------:|------------:|-------------:|------:|--------:|-----:|------------:|------------:|------------:|--------------------:|
|  Bonsai |    117.33 ns |   9.945 ns |    28.69 ns |    107.66 ns |  1.00 |    0.00 |   ** |           - |           - |           - |               144 B |
| Windsor | 10,582.30 ns | 397.848 ns | 1,166.82 ns | 10,333.75 ns | 95.36 |   22.75 | **** |      1.0000 |           - |           - |              4344 B |
| Autofac |  2,917.91 ns | 255.063 ns |   748.06 ns |  2,717.46 ns | 26.07 |    8.51 |  *** |      0.4000 |           - |           - |              2344 B |
|   Grace |     50.66 ns |   3.690 ns |    10.59 ns |     48.50 ns |  0.46 |    0.15 |    * |           - |           - |           - |               104 B |


Things we have tired or have yet to try:

* precomiling all delegates - **done** - positive improvement
* use a linked list instead of a stack - **done** - positive improvement
* remove dependency on the key dictionary (as much as possible) - **done** - positive improvement
* replace dictionary with immutable AVL - **done** - positive improvement
* get hash code for service keys, not using string concat - **done** - positive improvement
* track only disposables - **done** - positive improvement
* replace Concurrency with Out of the box concurrency collection - **skipped**
* look into interlock instead of the light weight read write lock - **skipped**
* look into direct IL - **done**, must be a better way to do it though
* try not to need any locking in the code - *under review*
* use of ServiceKey (annoyingly this is a core way to allow the service to be named)
