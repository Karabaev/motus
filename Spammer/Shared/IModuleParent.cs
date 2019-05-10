namespace Updater.Shared
{
    interface IModuleParent 
    {
        IModule[] DependentModules { get; set; }
    }
}
