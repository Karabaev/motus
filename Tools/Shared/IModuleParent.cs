namespace Tools.Shared
{
    interface IModuleParent 
    {
        IModule[] DependentModules { get; set; }
    }
}
