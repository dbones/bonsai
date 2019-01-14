namespace Bonsai.Registry
{
    public interface IModule
    {
        void Setup(ContainerBuilder builder);
    }
}