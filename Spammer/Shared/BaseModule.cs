namespace Updater.Shared
{
    using System;
    using System.Linq;
    using NLog;

    public class BaseModule : IModule
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public IModule ParentModule { get; set; }
        protected readonly Logger logger = LogManager.GetCurrentClassLogger();

        public virtual void Launch()
        {
            Console.WriteLine(this.ToString());
        }

        public override string ToString()
        {
            return string.Format("{0} - {1}", this.Index, this.Name);
        }

        public void Show()
        {
            Console.WriteLine("{0} ", this.ToString());
        }

        public static void SelectAndLaunchModule(IModule[] modules)
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("Выбери пункт:");
                    int switcher = int.Parse(Console.ReadLine());
                    IModule module = modules.FirstOrDefault(m => m.Index == switcher);
                    Console.WriteLine("Модуль \"{0}\" начал работу", module.Name);
                    module.Launch();
                    Console.WriteLine("Модуль \"{0}\" закончил работу \n", module.Name);
                    return;
                }
                catch (FormatException)
                {
                    Console.WriteLine("Неверный пункт меню");
                    continue;
                }
            }
        }
    }
}
