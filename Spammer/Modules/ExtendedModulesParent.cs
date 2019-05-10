namespace Updater.Modules
{
    using System;
    using System.Text;
    using Shared;

    public class ExtendedModulesParent : BaseModule, IModuleParent
    {
        public IModule[] DependentModules { get; set; }

        public ExtendedModulesParent(params IModule[] modules)
        {
            this.DependentModules = modules;
        }

        public override void Launch()
        {
            StringBuilder showString = new StringBuilder();

            foreach (var item in this.DependentModules)
                showString.AppendFormat("{0} ", item.ToString());

            Console.WriteLine(showString);
            BaseModule.SelectAndLaunchModule(this.DependentModules);
        }
    }
}
