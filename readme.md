## ![Bonsai](https://raw.githubusercontent.com/dbones/bonsai/master/images/bonsai-large.png "Bonsai IoC")


[![Build status](https://ci.appveyor.com/api/projects/status/wjj6iy88fdsl1sy7/branch/master?svg=true)](https://ci.appveyor.com/project/dbones/bonsai/branch/master) [![codecov](https://codecov.io/gh/dbones/bonsai/branch/master/graph/badge.svg)](https://codecov.io/gh/dbones/bonsai) ![](https://img.shields.io/nuget/v/Bonsai.Ioc.svg)


A small .NET IoC container, written in .NET Core 2.1.

Currently a work in progress. ( brand new :) )

# Features 

* Fluent Registration
* Open Generics
* Named instnances
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
    var service = _subject.Resolve<IService>();
}
```

