namespace Tools
{
    using NLog;
    using Modules;
    using Shared;

    public class Program
    {
        static int Main(string[] args)
        {
            RegisterModules();

            while (true)
            {
                foreach (var item in modules)
                    item.Show();

                BaseModule.SelectAndLaunchModule(modules);
            }
        }

        static private void RegisterModules()
        {
            IModule updater = new VideoMaterialUpdateModule
            {
                Index = 1,
                Name = "Проверка обновлений видеоматериалов"
            };
            IModule rangeDownloader = new VideoMaterialRangeDownloadModule
            {
                Index = 1,
                Name = "Загрузка по интервалу ID кинопоиска"
            };
            IModule urlDownloader = new VideoMaterialUrlDownloadModule
            {
                Index = 2,
                Name = "Загрузка по интервалу Url кинопоиска"
            };
            IModule moonDownloader = new MoonwalkDownloadModule
            {
                Index = 3,
                Name = "Загрузка базы мунвалка"
            };
            IModule downloadModulesParent = new ModulesParentBase
            {
                Index = 2,
                Name = "Загрузка видеоматериалов",
                DependentModules = new IModule[] { rangeDownloader, urlDownloader, moonDownloader }
            };

            IModule serialCheckerModule = new SerialCheckerModule
            {
                Index = 1,
                Name = "Проверка видеоматериалов на сериал"
            };
            IModule videoMaterialBlockedCleanModule = new VideoMaterialBlockedCleanModule
            {
                Index = 2,
                Name = "Чистка от заблокированных видеоматериалов"
            };
            IModule extendedModulesParent = new ModulesParentBase
            {
                Index = 3,
                Name = "Дополнительные модули",
                DependentModules = new IModule[] { serialCheckerModule, videoMaterialBlockedCleanModule }
            };

            modules = new IModule[] { updater, downloadModulesParent, extendedModulesParent };
        }

        private static IModule[] modules;
        private static Logger Logger { get; set; } = LogManager.GetCurrentClassLogger();
	}
}
