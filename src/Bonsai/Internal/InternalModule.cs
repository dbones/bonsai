namespace Bonsai.Internal
{
    using LifeStyles;
    using Registry;

    public class InternalModule : IModule
    {
        public void Setup(ContainerBuilder builder)
        {
            builder.Register<Scope>().As<Scope>().As<IScope>().As<IAdvancedScope>()
                .Scoped<PerScope>();
            
            
        }
    }
}