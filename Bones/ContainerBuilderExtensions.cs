namespace Bones
{
    using System;

    public static class ContainerBuilderExtensions
    {
        public static FluentBuilder<T> Register<T>(this ContainerBuilder builder) where T : class
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