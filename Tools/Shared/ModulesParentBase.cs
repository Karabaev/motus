namespace Tools.Shared
{
    using System;
    using System.Text;

    public class ModulesParentBase : BaseModule, IModuleParent
    {
        public IModule[] DependentModules { get; set; }

        public ModulesParentBase(params IModule[] modules)
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
