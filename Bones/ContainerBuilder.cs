namespace Bones
{
    using System;

    public class ContainerBuilder
    {
        public void SetupModules(params IModule[] modules)
        {
            foreach (var module in modules)
            {
                module.Setup(this);
            }
        }

        public IContainer Create()
        {
            throw new NotImplementedException();
        }

        public void RegisterContract(Registration registration)
        {
            throw new NotImplementedException();
        }
    }
}