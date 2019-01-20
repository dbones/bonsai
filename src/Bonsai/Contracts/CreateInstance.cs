namespace Bonsai.Contracts
{
    using Internal;

    /// <summary>
    /// a delegate which is responsible for creating an instance of an object
    /// </summary>
    public delegate object CreateInstance(IAdvancedScope scope, Contract contract, Contract parentContract = null);
}