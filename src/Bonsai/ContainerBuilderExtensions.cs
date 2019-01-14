﻿namespace Bonsai
{
    using System;
    using Registry;

    public static class ContainerBuilderExtensions
    {
        public static FluentBuilder<T> Register<T>(this ContainerBuilder builder) where T : class
        {
            var registration = new Registration();
            builder.RegisterContract(registration);
            return new FluentBuilder<T>(registration);
        }
        
        public static FluentBuilder<T> Register<T>(this ContainerBuilder builder, Func<Scope, T> ctor) where T : class
        {
            var registration = new Registration();
            
            builder.RegisterContract(registration);
            return new FluentBuilder<T>(registration);
        }
        
        public static FluentBuilder Register(this ContainerBuilder builder, Type serviceType)
        {
            var registration = new Registration();
            builder.RegisterContract(registration);
            return new FluentBuilder(registration, serviceType);
        }
    }
}