namespace Bones.Registry
{
    public interface IModule
    {
        void Setup(ContainerBuilder builder);
    }
}