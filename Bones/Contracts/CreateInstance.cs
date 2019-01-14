namespace Bones.Contracts
{
    /// <summary>
    /// a delegate which is responsible for creating an instance of an object
    /// </summary>
    /// <param name="currentScope">the current scope</param>
    public delegate object CreateInstance(IAdvancedScope currentScope);
}